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
using System.Linq;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.CreatorStudio.DocumentTabs;
using GZSkinsX.Api.Tabs;

using Windows.Foundation;

namespace GZSkinsX.Extensions.CreatorStudio.DocumentTabs;

[Shared, Export(typeof(IDocumentTabService))]
internal sealed class DocumentTabService : IDocumentTabService
{
    private readonly IEnumerable<Lazy<IDocumentTabProvider, DocumentTabProviderMetadataAttribute>> _mefProviders;
    private readonly Dictionary<string, DocumentTabProviderContext> _typeToProvider;
    private readonly ITabViewManager _tabViewManager;

    internal object UIObject => _tabViewManager.UIObject;

    public IDocumentTabContent? ActiveTab => (IDocumentTabContent?)_tabViewManager.ActiveTab;

    public IEnumerable<IDocumentTabContent> AllDocumentTabs => _tabViewManager.TabContents.OfType<IDocumentTabContent>();

    public event EventHandler<ActiveDocumentTabChangedEventArgs>? ActiveTabChanged;

    public event TypedEventHandler<IDocumentTabService, DocumentTabCollectionChangedEventArgs>? CollectionChanged;

    [ImportingConstructor]
    public DocumentTabService(
        [ImportMany] IEnumerable<Lazy<IDocumentTabProvider, DocumentTabProviderMetadataAttribute>> mefProviders)
    {
        _mefProviders = mefProviders;
        _typeToProvider = new Dictionary<string, DocumentTabProviderContext>();

        var tabViewService = AppxContext.Resolve<ITabViewService>();
        _tabViewManager = tabViewService.CreateTabViewManager("AD00CC08-C301-413A-AF87-1A0E3A0C86C9");
        _tabViewManager.ActiveTabChanged += OnActiveTabChanged;
        _tabViewManager.CollectionChanged += OnCollectionChanged;

        Initialize();
    }

    private void Initialize()
    {
        var separator = new char[] { ',', ';' };
        foreach (var item in _mefProviders)
        {
            var context = new DocumentTabProviderContext(item);
            var supportedExtensions = context.Metadata.SupportedExtensions;

            foreach (var ext in supportedExtensions.Split(separator))
            {
                _typeToProvider[ext] = context;
            }
        }
    }

    private void OnActiveTabChanged(object sender, ActiveTabChangedEventArgs e)
    {
        if (e.ActiveTab is IDocumentTabContent documentTab)
        {
            ActiveTabChanged?.Invoke(this, new(documentTab));
        }
    }

    private void OnCollectionChanged(ITabViewManager sender, TabCollectionChangedEventArgs args)
    {
        var @event = CollectionChanged;
        if (@event is null)
            return;

        @event.Invoke(this, new(
            args.AddedItems?.OfType<IDocumentTabContent>().ToArray(),
            args.RemovedItems?.OfType<IDocumentTabContent>().ToArray()));
    }

    internal IEnumerable<string> GetSupportedFileTypes()
    {
        foreach (var item in _typeToProvider.Keys)
        {
            yield return item;
        }
    }

    internal IDocumentTabProvider? GetDocumentTabProvider(string fileType)
    {
        return _typeToProvider.TryGetValue(fileType, out var ctx) ? ctx.Value : null;
    }

    internal void Add(IDocumentTabContent documentTab)
    {
        _tabViewManager.Add(documentTab);
    }

    public void Close(IDocumentTabContent documentTab)
    {
        if (documentTab is null)
        {
            throw new ArgumentNullException(nameof(documentTab));
        }

        _tabViewManager.Close(documentTab);
    }

    public void CloseActiveTab()
    {
        _tabViewManager.CloseActiveTab();
    }

    public void CloseAllButActiveTab()
    {
        _tabViewManager.CloseAllButActiveTab();
    }

    public void SetActiveTab(int index)
    {
        _tabViewManager.SetActiveTab(index);
    }

    public void SetActiveTab(IDocumentTabContent documentTab)
    {
        if (documentTab is null)
        {
            throw new ArgumentNullException(nameof(documentTab));
        }

        _tabViewManager.SetActiveTab(documentTab);
    }
}
