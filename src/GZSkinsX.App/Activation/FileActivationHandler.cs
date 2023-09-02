// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;
using System.Linq;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Activation;
using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Extension;
using GZSkinsX.Views;

using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Activation;

internal sealed class FileActivationHandler : IActivationHandler, IActivationHandler2
{
    public async Task<bool> CanHandleAsync(IActivatedEventArgs args)
    {
        if (await AppxContext.GameService.TryGetRootFolderAsync() is null)
        {
            return false;
        }

        if (args.PreviousExecutionState is ApplicationExecutionState.Running)
        {
            return false;
        }

        if (args.Kind is not ActivationKind.File || args is not FileActivatedEventArgs e)
        {
            return false;
        }

        if (e.Files.Any() is false)
        {
            return false;
        }

        return true;
    }

    public async Task HandleAsync(IActivatedEventArgs args)
    {
        if (args is not FileActivatedEventArgs e)
        {
            return;
        }

        var modFiles = e.Files.OfType<StorageFile>();
        if (modFiles.Any() is false)
        {
            return;
        }

        await AppxContext.MyModsService.ImportModsAsync(modFiles);

        if (AppxContext.AppxWindow.MainWindow.Content is Frame rootFrame)
        {
            if (rootFrame.Content is MainPage mainPage)
            {
                await mainPage.ViewModel.OnRefreshAsync();
            }
        }

        // Just handle once
        AppxContext.ActivationService.UnregisterHandler(this);
    }

    bool IActivationHandler.CanHandle(IActivatedEventArgs args)
    {
        return false;
    }

    [Shared, ExportAdvanceExtension]
    [AdvanceExtensionMetadata(Order = double.MinValue)]
    private sealed class AutoLoaded : IAdvanceExtension
    {
        public AutoLoaded()
        {
            AppxContext.ActivationService.RegisterHandler(new FileActivationHandler());
        }
    }
}
