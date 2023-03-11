// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;

using GZSkinsX.Api;
using GZSkinsX.Api.Shell;

using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Appx.Preload;

/// <summary>
/// 
/// </summary>
[Shared, ExportViewElement]
[ViewElementMetadata(Guid = ViewElementConstants.Preload_Guid, PageType = typeof(PreloadPage))]
internal sealed class ExportPreloadPage : IViewElement
{
    /// <summary>
    /// 
    /// </summary>
    private readonly IViewManagerService _viewManagerService;

    /// <summary>
    /// 
    /// </summary>
    [ImportingConstructor]
    public ExportPreloadPage(IViewManagerService viewManagerService)
    {
        _viewManagerService = viewManagerService;
    }

    /// <inheritdoc/>
    public async Task OnInitializeAsync(Page viewElement)
    {
        var random = new Random();
        var waitingTime = random.Next(1000, 2000);
        Debug.WriteLine($"AppxPreload: Waiting Time = {waitingTime}");
        await Task.Delay(waitingTime);

        _viewManagerService.NavigateTo(ViewElementConstants.StartUp_Guid);
    }

    /// <inheritdoc/>
    public async Task OnNavigatingAsync(WindowFrameNavigateEventArgs args)
    {
        await Task.CompletedTask;
    }
}
