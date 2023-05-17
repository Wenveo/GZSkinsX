// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Composition;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.CreatorStudio.DocumentTabs;

namespace GZSkinsX.Extensions.CreatorStudio.DocumentTabs;

[Shared, Export(typeof(IDocumentTabLoader))]
public sealed class DocumentTabLoader : IDocumentTabLoader
{
    private readonly DocumentTabService _documentTabService;

    public DocumentTabLoader()
    {
        _documentTabService = (DocumentTabService)AppxContext.Resolve<IDocumentTabService>();
    }

    public IDocumentTabContent? Load(DocumentInfo documentInfo)
    {
        var provider = _documentTabService.GetDocumentTabProvider(documentInfo.FileType);
        if (provider is not null)
        {
            var documentTab = provider.Create(documentInfo);
            _documentTabService.Add(documentTab);

            return documentTab;
        }

        return null;
    }
}
