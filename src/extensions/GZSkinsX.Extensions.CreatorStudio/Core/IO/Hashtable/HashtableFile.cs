// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Hashing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;

using GZSkinsX.SDK.Diagnostics;

namespace GZSkinsX.Extensions.CreatorStudio.Core.IO.Hashtable;

internal sealed class HashtableFile : IDisposable
{
    private sealed class HashtableEntry(long dataOffset, string? name)
    {
        public long DataOffset = dataOffset;
        public string? Name = name;
    }

    private readonly struct HashtableSegment(long position)
    {
        public readonly long Position = position;
        public readonly Dictionary<ulong, HashtableEntry> Entries = new();
    }

#pragma warning disable format
    private const int   HashBit             = 39;
    private const ulong HashKey             = (1UL << HashBit) - 1;
    private const int   MagicCode           = 0x54_48_53_41;
    private const int   MaxSegmentLength    = 16777216;
#pragma warning restore format

    private readonly Dictionary<ulong, HashtableEntry> _entries;
    private Stream? _stream;

    public HashtableFile()
    {
        _entries = new Dictionary<ulong, HashtableEntry>();
    }

    public HashtableFile(Stream inputStream) : this()
    {
        Guard.IsNotNull(inputStream);
        Guard.CanRead(inputStream);
        Guard.CanSeek(inputStream);

        using var br = new BinaryReader(inputStream, Encoding.UTF8, true);

        var magicCode = br.ReadInt32();
        Guard.IsEqualTo(magicCode, MagicCode);

        var count = br.ReadInt32();
        var segmentMDOffset = br.ReadInt32();
        br.BaseStream.Seek(segmentMDOffset, SeekOrigin.Begin);

        for (var i = 0; i < count; i++)
        {
            ReadHashtableSegment(br);
        }

        _stream = inputStream;
    }

    private void ReadHashtableSegment(BinaryReader br)
    {
        var count = br.ReadInt32();
        var position = br.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var hashData = br.ReadUInt64();

            var offset = Convert.ToInt64(hashData >> HashBit);
            var hash = hashData & HashKey;

            _entries.Add(hash, new(position + offset, null));
        }
    }

    private string ReadText(long dataOffset)
    {
        Debug2.Assert(_stream is not null);
        _stream.Seek(dataOffset, SeekOrigin.Begin);

        var length = _stream.Read<short>();
        var buffer = ArrayPool<byte>.Shared.Rent(length);

        var bytesOffset = 0;
        try
        {
            do
            {
                var bytesRead = _stream.Read(buffer, bytesOffset, length - bytesOffset);
                if (bytesRead == 0)
                {
                    break;
                }

                bytesOffset += bytesRead;
            }
            while (bytesOffset < length);

            return StringPool.Shared.GetOrAdd(new ReadOnlySpan<byte>(buffer, 0, length), Encoding.UTF8);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public void Insert(ulong hash, string name)
    {
        Guard.IsNotNullOrEmpty(name);

        hash &= HashKey;
        if (_entries.TryGetValue(hash, out var entry))
        {
            entry.Name = name;
        }
        else
        {
            _entries.Add(hash, new(0, name));
        }
    }

    public string GetNameOrHexString(ulong hash)
    {
        if (_entries.TryGetValue(hash & HashKey, out var entry))
        {
            return entry.Name ??= ReadText(entry.DataOffset);
        }

        return hash.ToHexString();
    }

    public bool TryGetName(ulong hash, [NotNullWhen(true)] out string? name)
    {
        if (_entries.TryGetValue(hash & HashKey, out var entry) &&
            entry.Name is null && _stream is not null)
        {
            entry.Name = ReadText(entry.DataOffset);
        }

        return (name = entry.Name) is not null;
    }

    public void Write(Stream outputStream)
    {
        Guard.IsNotNull(outputStream);
        Guard.CanSeek(outputStream);
        Guard.CanWrite(outputStream);

        using var bw = new BinaryWriter(outputStream, Encoding.UTF8, true);

        bw.Write(MagicCode);

        if (_entries.Count == 0)
        {
            bw.Write(0);
            bw.Write((int)outputStream.Position);
            return;
        }

        var segmentMDOffset = (int)outputStream.Position;

        var orderedArray = _entries.OrderBy(a => a.Key).ToArray();
        var sharedBuffer = ArrayPool<byte>.Shared.Rent(262);
        var checksumToHash = new Dictionary<ulong, ulong>();

        var hashtableSegments = new List<HashtableSegment> { new(outputStream.Position + sizeof(int)) };
        var currentSegment = hashtableSegments[0];

        var bytesCount = 0;

        try
        {
            for (var i = 0; i < orderedArray.Length; i++)
            {
                ReadOnlySpan<byte> rawData;
                var pair = orderedArray[i];
                if (pair.Value.Name is null)
                {
                    Debug2.Assert(_stream is not null);
                    _stream.Seek(pair.Value.DataOffset, SeekOrigin.Begin);

                    var length = _stream.Read<short>();
                    if (length + sizeof(short) > sharedBuffer.Length)
                    {
                        ArrayPool<byte>.Shared.Return(sharedBuffer);
                        sharedBuffer = ArrayPool<byte>.Shared.Rent(length + sizeof(short));
                    }

                    MemoryMarshal.Write(sharedBuffer, ref length);

                    var bytesOffset = sizeof(short);
                    do
                    {
                        var bytesRead = _stream.Read(sharedBuffer, bytesOffset, length - bytesOffset);
                        if (bytesRead == 0)
                        {
                            break;
                        }

                        bytesOffset += bytesRead;
                    }
                    while (bytesOffset < length);

                    rawData = new ReadOnlySpan<byte>(sharedBuffer, 0, length + sizeof(short));
                }
                else
                {
                    var dataLength = Encoding.UTF8.GetByteCount(pair.Value.Name);
                    if (dataLength + sizeof(short) > sharedBuffer.Length)
                    {
                        ArrayPool<byte>.Shared.Return(sharedBuffer);
                        sharedBuffer = ArrayPool<byte>.Shared.Rent(dataLength + sizeof(short));
                    }

                    MemoryMarshal.Write(sharedBuffer, ref dataLength);

                    var writeCount = Encoding.UTF8.GetBytes(pair.Value.Name, 0, pair.Value.Name.Length, sharedBuffer, sizeof(short));
                    Debug2.Assert(writeCount <= dataLength);

                    rawData = new ReadOnlySpan<byte>(sharedBuffer, 0, writeCount + sizeof(short));
                }

                var checksum = XxHash3.HashToUInt64(rawData);
                if (checksumToHash.TryGetValue(checksum, out var hashOfDuplicateEntry))
                {
                    var b = false;
                    foreach (var segment in hashtableSegments)
                    {
                        if (segment.Entries.TryGetValue(hashOfDuplicateEntry, out var duplicateEntry))
                        {
                            pair.Value.DataOffset = duplicateEntry.DataOffset;
                            segment.Entries.Add(pair.Key, pair.Value);

                            b = true;
                            break;
                        }
                    }

                    Debug2.Assert(b);
                }
                else
                {
                    if (bytesCount + rawData.Length > MaxSegmentLength)
                    {
                        hashtableSegments.Add(currentSegment = new(outputStream.Position));

                        // Reset
                        bytesCount = 0;
                    }

                    pair.Value.DataOffset = bytesCount;
                    bw.Write(rawData.ToArray());

                    currentSegment.Entries.Add(pair.Key, pair.Value);
                    bytesCount += rawData.Length;
                }
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(sharedBuffer);
        }

        var segmentOffset = (int)bw.BaseStream.Position;
        for (var j = 0; j < hashtableSegments.Count; j++)
        {
            var hashtableSegment = hashtableSegments[j];
            bw.Write(hashtableSegment.Entries.Count);
            bw.Write((int)hashtableSegment.Position);

            foreach (var item in hashtableSegments[j].Entries)
            {
                bw.Write(item.Key + Convert.ToUInt64(item.Value.DataOffset << HashBit));
            }
        }

        bw.Seek(segmentMDOffset, SeekOrigin.Begin);
        bw.Write(hashtableSegments.Count);
        bw.Write(segmentOffset);

        bw.Flush();
    }


    public void Dispose()
    {
        if (_stream is not null)
        {
            _stream.Dispose();
            _stream = null;

            GC.SuppressFinalize(this);
        }
    }

    public static HashtableFile ParseCDTBHashtable(IEnumerable<string> strings)
    {
        var hashtableFile = new HashtableFile();

        foreach (var line in strings)
        {
            var indexOfWhiteSpace = line.IndexOf(' ');

            var hash = ulong.Parse(line[..indexOfWhiteSpace], System.Globalization.NumberStyles.HexNumber);
            var name = line[++indexOfWhiteSpace..];

            hashtableFile.Insert(hash, name);
        }

        return hashtableFile;
    }
}
