// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;

namespace GZSkinsX.Extensions.CreatorStudio.Wad.Parser;

internal sealed class WadFile : IDisposable
{
    private static readonly ZstdNet.Decompressor s_zstd_decompressor = new();
    private readonly ReadOnlyMemory<WadFileSubChunkInfo>? _subChunkMap;
    private readonly Stream _stream;
    private bool _isDisposed;

    public Dictionary<ulong, WadFileChunk> FileChunks { get; }

    public Version FileVersion { get; }

    public WadFile(Stream inputStream)
    {
        using var br = new BinaryReader(_stream = inputStream, Encoding.UTF8, true);

        var byt_header = br.ReadBytes(2);
        if (byt_header[0] is not 0x52 && byt_header[1] is not 0x57)
        {
            throw new InvalidDataException($"Invalid Wad file header: 0x{byt_header[0]:X}_{byt_header[1]:X}");
        }

        var byt_major = br.ReadByte();
        var byt_minor = br.ReadByte();

        switch (byt_major)
        {
            case 1:
                br.BaseStream.Seek(4, SeekOrigin.Current);
                break;
            case 2:
                br.BaseStream.Seek(1 + 83 + 8 + 4, SeekOrigin.Current);
                break;
            case 3:
                br.BaseStream.Seek(256 + 8, SeekOrigin.Current);
                break;
            default:
                throw new InvalidDataException($"Invalid Wad file version: v{byt_major}.{byt_minor}");
        };

        FileChunks = new();
        FileVersion = new(byt_major, byt_minor);

        var chunkCount = br.ReadInt32();
        for (var i = 0; i < chunkCount; i++)
        {
            var chunk = new WadFileChunk(br, byt_major);
            FileChunks.Add(chunk.XxHash, chunk);
        }

        if (byt_major > 2)
        {
            var expectedChunkCount = FileChunks.Values.Select(
                item => item.FirstChunkIndex + item.SubChunkCount).Max();

            var possibleChoice = FileChunks.Values.FirstOrDefault(
                item => item.UncompressedSize % 16 == 0 && item.UncompressedSize / 16 == expectedChunkCount);

            if (possibleChoice is not null)
            {
                _subChunkMap = ReadOnlyMemoryExtensions.Cast<byte, WadFileSubChunkInfo>(ReadDataImpl(possibleChoice, true).Memory);
            }
        }
    }

    private MemoryOwner<byte> ReadDataCore(WadFileChunk chunk)
    {
        var position = _stream.Seek(chunk.DataOffset, SeekOrigin.Begin);
        Debug.Assert(position == chunk.DataOffset);

        var memoryOwner = MemoryOwner<byte>.Allocate(chunk.CompressedSize);
        var totalRead = _stream.Read(memoryOwner.Span);
        Debug.Assert(totalRead == chunk.CompressedSize);

        return memoryOwner;
    }

    private MemoryOwner<byte> ReadOriginData(WadFileChunk chunk)
    {
        using var compressedData = ReadDataCore(chunk);
        using var decompressionStream = chunk.Type switch
        {
            WadFileChunkType.Gzip => new GZipStream(compressedData.AsStream(), CompressionMode.Decompress),
            WadFileChunkType.Link => compressedData.Slice(4, compressedData.Length - 4).AsStream(),
            WadFileChunkType.Zstd => new ZstdNet.DecompressionStream(compressedData.AsStream()),
            WadFileChunkType.ZstdChunked => ReadZstdChunk(chunk, compressedData).AsStream(),
            _ => throw new InvalidOperationException()
        };

        var descompressedData = MemoryOwner<byte>.Allocate(chunk.UncompressedSize);
        decompressionStream.Read(descompressedData.Span);

        return descompressedData;
    }

    private MemoryOwner<byte> ReadZstdChunk(WadFileChunk chunk, MemoryOwner<byte> source)
    {
        if (_subChunkMap.HasValue is false)
        {
            throw new InvalidOperationException("The subChunkMap is null!");
        }

        var decompressedData = MemoryOwner<byte>.Allocate(chunk.UncompressedSize);
        var subChunks = _subChunkMap.Value.Slice(chunk.FirstChunkIndex, chunk.SubChunkCount).ToArray();

        var srcOffset = 0;
        var dstOffset = 0;

        for (var i = 0; i < subChunks.Length; i++)
        {
            var subChunkInfo = subChunks[i];
            var compressedSize = subChunkInfo.CompressedSize;
            var uncompressedSize = subChunkInfo.UncompressedSize;

            if (compressedSize == uncompressedSize)
            {
                source.Span.Slice(srcOffset, compressedSize)
                    .CopyTo(decompressedData.Span[srcOffset..]);
            }
            else
            {
                s_zstd_decompressor
                    .Unwrap(source.Span.Slice(srcOffset, compressedSize))
                    .CopyTo(decompressedData.Span[dstOffset..]);
            }

            srcOffset += compressedSize;
            dstOffset += uncompressedSize;
        }

        Debug.Assert(srcOffset == chunk.CompressedSize);
        Debug.Assert(dstOffset == chunk.UncompressedSize);

        return decompressedData;
    }

    private MemoryOwner<byte> ReadDataImpl(WadFileChunk chunk, bool decompress)
    {
        if (chunk.Type == WadFileChunkType.None || !decompress)
            return ReadDataCore(chunk);

        return ReadOriginData(chunk);
    }

    public MemoryOwner<byte> ReadData(WadFileChunk chunk, bool decompress)
    {
        if (chunk is null)
        {
            throw new ArgumentNullException(nameof(chunk));
        }

        return ReadDataImpl(chunk, decompress);
    }

    public override string ToString()
    {
        return $"Version = v{FileVersion.Major}.{FileVersion.Minor}, FileCount = {FileChunks.Count}";
    }

    void IDisposable.Dispose()
    {
        if (_isDisposed is false)
        {
            _stream?.Dispose();
            _isDisposed = true;

            GC.SuppressFinalize(this);
        }
    }
}
