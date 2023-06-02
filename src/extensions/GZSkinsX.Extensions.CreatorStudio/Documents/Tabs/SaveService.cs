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

using GZSkinsX.Extensions.CreatorStudio.Contracts.Documents;
using GZSkinsX.Extensions.CreatorStudio.Contracts.Documents.Tabs;

namespace GZSkinsX.Extensions.CreatorStudio.Documents.Tabs;

[Shared, Export(typeof(ISaveService))]
internal sealed class SaveService : ISaveService
{
    private readonly IEnumerable<Lazy<ITabSaverProvider, TabSaverProviderMetadataAttribute>> _mefProviders;
    private readonly IDocumentTabService _documentTabService;

    private readonly Dictionary<Guid, TabSaverProviderContext> _guidToProvider;
    private readonly Dictionary<IDocumentKey, WeakReference> _cachedTabSavers;

    [ImportingConstructor]
    public SaveService(
        [ImportMany] IEnumerable<Lazy<ITabSaverProvider, TabSaverProviderMetadataAttribute>> mefProviders,
        IDocumentTabService documentTabService)
    {
        _mefProviders = mefProviders;
        _documentTabService = documentTabService;
        _documentTabService.CollectionChanged += OnCollectionChanged;

        _guidToProvider = new();
        _cachedTabSavers = new();

        Initialize();
    }

    private void OnCollectionChanged(IDocumentTabService sender, DocumentTabCollectionChangedEventArgs args)
    {
        if (args.RemovedItems is not null)
        {
            foreach (var item in args.RemovedItems)
            {
                _cachedTabSavers.Remove(item.Document.Key);
            }
        }
    }

    private void Initialize()
    {
        _guidToProvider.Clear();

        foreach (var item in _mefProviders)
        {
            var typedGuidString = item.Metadata.TypedGuid;
            var b = Guid.TryParse(typedGuidString, out var typedGuid);
            Debug.Assert(b, $"TabSaverProvider: Couldn't parse TypedGuid property: '{typedGuidString}'");
            if (!b)
                continue;

            _guidToProvider[typedGuid] = new TabSaverProviderContext(item);
        }
    }

    private ITabSaver? GetTabSaver(IDocumentTab tab)
    {
        if (tab is null)
        {
            return null;
        }

        ITabSaver? tabSaver;
        var key = tab.Document.Key;

        if (_cachedTabSavers.TryGetValue(key, out var weakTabSaver))
        {
            tabSaver = weakTabSaver.Target as ITabSaver;
            if (tabSaver is not null)
            {
                return tabSaver;
            }
        }

        if (_guidToProvider.TryGetValue(tab.Document.Info.TypedGuid, out var ctx))
        {
            tabSaver = ctx.Value.Create(tab);
            _cachedTabSavers[key] = new WeakReference(tabSaver);

            return tabSaver;
        }

        return null;
    }

    public bool CanSave(IDocumentTab tab)
    {
        var tabSaver = GetTabSaver(tab);
        return tabSaver is not null && tabSaver.CanSave;
    }

    public void Save(IDocumentTab tab)
    {
        var tabSaver = GetTabSaver(tab);
        if (tabSaver is not null && tabSaver.CanSave)
        {
            tabSaver.Save();
        }
    }

    public void SaveAs(IDocumentTab tab)
    {
        var tabSaver = GetTabSaver(tab);
        if (tabSaver is not null && tabSaver.CanSave)
        {
            tabSaver.SaveAs();
        }
    }
}
