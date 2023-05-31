// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.IO;

namespace GZSkinsX.Extensions.CreatorStudio.Wad.Parser;

internal sealed class WadFileChunk : IEquatable<WadFileChunk>
{
    public ulong XxHash { get; }

    public int DataOffset { get; }

    public int CompressedSize { get; }

    public int UncompressedSize { get; }

    public WadFileChunkType Type { get; }

    public bool IsDuplicated { get; }

    public ushort FirstChunkIndex { get; }

    public int SubChunkCount { get; }

    public ulong Checksum { get; set; }

    public WadFileChunk(BinaryReader br, byte major)
    {
        XxHash = br.ReadUInt64();
        DataOffset = br.ReadInt32();
        CompressedSize = br.ReadInt32();
        UncompressedSize = br.ReadInt32();

        var byt_type = br.ReadByte();
        SubChunkCount = byt_type >> 4;
        Type = (WadFileChunkType)(byt_type & 0xF);

        IsDuplicated = br.ReadBoolean();
        FirstChunkIndex = br.ReadUInt16();

        if (major >= 2)
        {
            Checksum = br.ReadUInt64();
        }
    }

    public void Write(BinaryWriter bw)
    {
        bw.Write(XxHash);
        bw.Write(DataOffset);
        bw.Write(CompressedSize);
        bw.Write(UncompressedSize);
        bw.Write((byte)Type);
        bw.Write(IsDuplicated);
        bw.Write(FirstChunkIndex);
        bw.Write(Checksum);
    }

    public bool Equals(WadFileChunk? other)
    {
        return other != null && other.XxHash == XxHash;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as WadFileChunk);
    }

    public override int GetHashCode()
    {
        return XxHash.GetHashCode();
    }
}
