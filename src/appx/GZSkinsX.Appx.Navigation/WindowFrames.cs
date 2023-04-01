// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;
using System.Threading.Tasks;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Navigation;
using GZSkinsX.Api.WindowManager;
using GZSkinsX.DotNet.Diagnostics;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace GZSkinsX.Appx.Navigation;

[Shared, ExportWindowFrame]
[WindowFrameMetadata(Guid = WindowFrameConstants.NavigationRoot_Guid, PageType = typeof(NavigationRootPage))]
internal sealed class ExportNavigationRootFrame : IWindowFrame
{
    private readonly INavigationService _navigationService;
    private readonly IAppxTitleBar _appxTitleBar;

    [ImportingConstructor]
    public ExportNavigationRootFrame(INavigationService navigationService, IAppxTitleBar appxTitleBar)
    {
        _navigationService = navigationService;
        _appxTitleBar = appxTitleBar;
    }

    public async Task OnNavigateFromAsync()
    {
        await Task.CompletedTask;
    }

    public async Task OnNavigateToAsync(NavigationEventArgs args)
    {
        var navRootPage = args.Content as NavigationRootPage;
        Debug2.Assert(navRootPage is not null);
        navRootPage.OnLoaded(_navigationService, _appxTitleBar);

        await Task.CompletedTask;
    }

    public async Task OnNavigatingAsync(WindowFrameNavigateEventArgs args)
    {
        await Task.CompletedTask;
    }
}
