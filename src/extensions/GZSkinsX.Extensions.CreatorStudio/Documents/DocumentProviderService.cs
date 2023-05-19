// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Linq;

using GZSkinsX.Api.CreatorStudio.Documents;

namespace GZSkinsX.Extensions.CreatorStudio.Documents;

[Shared, Export(typeof(DocumentProviderService))]
internal sealed class DocumentProviderService
{
    private readonly IEnumerable<Lazy<IDocumentProvider, DocumentProviderMetadataAttribute>> _mefProviders;
    private readonly Dictionary<Guid, DocumentProviderContext> _typedToProvider;

    private IEnumerable<SupportedDocumentType>? _allSupportedDocumentTypes;
    private IEnumerable<string>? _allSuppportedExtensions;

    public IEnumerable<SupportedDocumentType> AllSupportedDocumentTypes
    {
        get => _allSupportedDocumentTypes ??= GetAllSuppportedDocumentTypes();
    }

    public IEnumerable<string> AllSuppportedExtensions
    {
        get => _allSuppportedExtensions ??= GetAllSuppportedExtensions();
    }

    [ImportingConstructor]
    public DocumentProviderService([ImportMany] IEnumerable<Lazy<IDocumentProvider, DocumentProviderMetadataAttribute>> mefProviders)
    {
        _mefProviders = mefProviders;
        _typedToProvider = new Dictionary<Guid, DocumentProviderContext>();

        Initialize();
    }

    private void Initialize()
    {
        _typedToProvider.Clear();

        foreach (var item in _mefProviders)
        {
            var typedGuidString = item.Metadata.TypedGuid;
            var b = Guid.TryParse(typedGuidString, out var typedGuid);
            Debug.Assert(b, $"DocumentProvider: Couldn't parse TypedGuid property: '{typedGuidString}'");
            if (!b)
                continue;

            _typedToProvider[typedGuid] = new DocumentProviderContext(item);
        }
    }

    public IDocumentProvider? GetProvider(string typedGuidString)
    {
        if (Guid.TryParse(typedGuidString, out var typedGuid))
        {
            if (_typedToProvider.TryGetValue(typedGuid, out var ctx))
            {
                return ctx.Value;
            }
        }

        return null;
    }

    public IDocumentProvider? GetProvider(Guid typedGuid)
    {
        if (_typedToProvider.TryGetValue(typedGuid, out var ctx))
        {
            return ctx.Value;
        }

        return null;
    }

    public IEnumerable<SupportedDocumentType> GetAllSuppportedDocumentTypes()
    {
        foreach (var item in _typedToProvider)
        {
            yield return new SupportedDocumentType(item.Value, item.Key);
        }
    }

    public IEnumerable<string> GetAllSuppportedExtensions()
    {
        foreach (var item in AllSupportedDocumentTypes)
        {
            foreach (var ext in item.SupportedExtensions)
            {
                yield return ext;
            }
        }
    }

    public bool TryGetTypedGuid(string fileType, out Guid typedGuid)
    {
        foreach (var documentType in AllSupportedDocumentTypes)
        {
            var item = documentType.SupportedExtensions.FirstOrDefault(
                a => StringComparer.OrdinalIgnoreCase.Equals(a, fileType));

            if (item is not null)
            {
                typedGuid = documentType.TypedGuid;
                return true;
            }
        }

        typedGuid = Guid.Empty;
        return false;
    }
}
