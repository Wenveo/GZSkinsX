// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Activation;
using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.WindowManager;
using GZSkinsX.Views;

using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;

using Windows.Storage;

namespace GZSkinsX.Activation;

internal sealed class MainActivationHandler : IActivationHandler
{
    private static readonly Lazy<MainActivationHandler> s_lazy = new(() => new());

    public static MainActivationHandler Instance => s_lazy.Value;

    public bool CanHandle(AppActivationArguments args)
    {
        if (args.Kind is not ExtendedActivationKind.File ||
            args.Data is not IReadOnlyList<IStorageItem> e)
        {
            return false;
        }

        if (e.Any(a => Path.GetExtension(a.Name) == ".lolgezi") is false)
        {
            return false;
        }

        return true;
    }

    public async Task HandleAsync(AppActivationArguments args)
    {
        if (args.Kind is not ExtendedActivationKind.File ||
            args.Data is not IReadOnlyList<IStorageItem> e)
        {
            return;
        }

        var modFiles = e.OfType<StorageFile>().Where(a => Path.GetExtension(a.Name) == ".lolgezi");
        if (modFiles.Any() is false)
        {
            return;
        }

        if (AppxContext.AppxWindow.MainWindow.Content is Frame rootFrame)
        {
            if (rootFrame.Content is MainPage mainPage)
            {
                await mainPage.ViewModel.ImportAsync(modFiles);
            }
            else
            {
                AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Main_Guid, modFiles);
            }
        }
    }
}
