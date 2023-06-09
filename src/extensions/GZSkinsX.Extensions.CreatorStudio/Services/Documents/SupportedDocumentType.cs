// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;

namespace GZSkinsX.Extensions.CreatorStudio.Services.Documents;

internal sealed class SupportedDocumentType
{
    private IEnumerable<string>? _supportedExtensions;

    public IEnumerable<string> SupportedExtensions
    {
        get => _supportedExtensions ??= GetSupportedExtensions();
    }

    public DocumentProviderContext Context { get; }

    public Guid TypedGuid { get; }

    public SupportedDocumentType(DocumentProviderContext context, Guid typedGuid)
    {
        Context = context;
        TypedGuid = typedGuid;
    }

    private IEnumerable<string> GetSupportedExtensions()
    {
        var str = Context.Metadata.SupportedExtensions;
        if (str.IndexOf(';') == -1)
        {
            yield return str;
        }
        else
        {
            foreach (var ext in str.Split(';'))
            {
                yield return ext;
            }
        }
    }
}
