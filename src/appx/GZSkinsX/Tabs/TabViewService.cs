// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Collections.Generic;
using System.Composition;
using System.Diagnostics.CodeAnalysis;

using GZSkinsX.Api.Tabs;

using Microsoft.UI.Xaml.Controls;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace GZSkinsX.Tabs;

[Shared, Export(typeof(ITabViewService))]
internal sealed class TabViewService : ITabViewService
{
    private readonly Dictionary<string, ITabViewManager> _guidToManager;

    public IEnumerable<ITabViewManager> AllTabViewManager => _guidToManager.Values;

    public TabViewService()
    {
        _guidToManager = new Dictionary<string, ITabViewManager>();
    }

    private TabView CreateDefaultTabView()
    {
        return new TabView
        {
            IsAddTabButtonVisible = false,
            TabWidthMode = TabViewWidthMode.Equal,
            VerticalAlignment = VerticalAlignment.Stretch,
            KeyboardAcceleratorPlacementMode = KeyboardAcceleratorPlacementMode.Hidden
        };
    }

    private ITabViewManager CoreceTabViewManager(
        [NotNull] TabViewManagerOptions options,
        [MaybeNull] TabView? targetElement)
    {
        var manager = new TabViewManager(targetElement ?? CreateDefaultTabView(), options);
        _guidToManager.Add(options.TabViewManagerGuid, manager);
        return manager;
    }

    public ITabViewManager CreateTabViewManager(string guidString)
    {
        return CoreceTabViewManager(new TabViewManagerOptions(guidString), null);
    }

    public ITabViewManager CreateTabViewManager(string guidString, TabView targetElement)
    {
        return CoreceTabViewManager(new TabViewManagerOptions(guidString), targetElement);
    }

    public ITabViewManager CreateTabViewManager(TabViewManagerOptions options)
    {
        return CoreceTabViewManager(options, null);
    }

    public ITabViewManager CreateTabViewManager(TabViewManagerOptions options, TabView targetElement)
    {
        return CoreceTabViewManager(options, targetElement);
    }
}
