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

using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;

using Windows.ApplicationModel.Activation;
using Windows.Storage;

namespace GZSkinsX.Activation;

internal sealed class FileActivationHandler : IActivationHandler
{
    public bool CanHandle(AppActivationArguments args)
    {
        if (AppxContext.GameService.RootDirectory is null)
        {
            return false;
        }

        if (args.Kind is not ExtendedActivationKind.File)
        {
            return false;
        }

        if (args.Data is not FileActivatedEventArgs e)
        {
            return false;
        }

        if (e.Files.Any() is false)
        {
            return false;
        }

        return true;
    }

    public async Task HandleAsync(AppActivationArguments args)
    {
        if (args.Kind is not ExtendedActivationKind.File)
        {
            return;
        }

        if (args.Data is not FileActivatedEventArgs e)
        {
            return;
        }

        var modFiles = e.Files.OfType<StorageFile>();
        if (modFiles.Any() is false)
        {
            return;
        }

        await AppxContext.MyModsService.ImportModsAsync(modFiles.Select(a => a.Path));
        AppxContext.AppxWindow.MainWindow.DispatcherQueue.TryEnqueue(async () =>
        {
            if (AppxContext.AppxWindow.MainWindow.Content is Frame rootFrame)
            {
                if (rootFrame.Content is MainPage mainPage)
                {
                    await mainPage.ViewModel.OnRefreshAsync();
                }
            }
        });

        await Task.CompletedTask;

        // Just handle once
        AppxContext.ActivationService.UnregisterHandler(this);
    }

    [Shared, ExportImplicitExtension]
    [ImplicitExtensionMetadata(Order = double.MinValue)]
    private sealed class AutoLoaded : IImplicitExtension
    {
        public AutoLoaded()
        {
            AppxContext.ActivationService.RegisterHandler(new FileActivationHandler());
        }
    }
}
