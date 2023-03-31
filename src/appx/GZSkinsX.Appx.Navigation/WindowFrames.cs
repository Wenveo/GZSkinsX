// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;
using System.Threading.Tasks;

using GZSkinsX.Api.Navigation;
using GZSkinsX.Api.WindowManager;

using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Appx.Navigation;

[Shared, ExportWindowFrame]
[WindowFrameMetadata(Guid = WindowFrameConstants.NavigationRoot_Guid, PageType = typeof(NavigationRootPage))]
internal sealed class ExportNavigationRootFrame : IWindowFrame
{
    private readonly INavigationService _navigationService;

    [ImportingConstructor]
    public ExportNavigationRootFrame(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public async Task OnInitializeAsync(Page viewElement)
    {
        var navRootPage = (NavigationRootPage)viewElement;
        navRootPage.Initialize(_navigationService);

        await Task.CompletedTask;
    }

    public async Task OnNavigatingAsync(WindowFrameNavigateEventArgs args)
    {
        await Task.CompletedTask;
    }
}
