// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CommunityToolkit.WinUI;

using GZSkinsX.Contracts.Activation;
using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Navigation;
using GZSkinsX.Contracts.WindowManager;

using Microsoft.Windows.AppLifecycle;

using Windows.ApplicationModel.Activation;
using Windows.Storage;

namespace GZSkinsX.Appx.MainApp.Activations;

internal sealed class MainActivationHandler : IActivationHandler
{
    private static readonly Lazy<MainActivationHandler> s_lazy = new(() => new());

    public static MainActivationHandler Instance => s_lazy.Value;

    public bool CanHandle(AppActivationArguments args)
    {
        if (args.Kind is not ExtendedActivationKind.File)
        {
            return false;
        }

        if (args.Data is not FileActivatedEventArgs e)
        {
            return false;
        }

        if (e.Files.Any(a => Path.GetExtension(a.Name) == ".lolgezi") is false)
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

        var modFiles = e.Files.OfType<StorageFile>().Where(a => Path.GetExtension(a.Name) == ".lolgezi");
        if (modFiles.Any() is false)
        {
            return;
        }

        await AppxContext.DispatcherQueue.EnqueueAsync(() =>
        {
            AppxContext.WindowManagerService.NavigateTo(
                WindowFrameConstants.Main_Guid,
                new NavigationViewNavigateArgs
                {
                    NavItemGuid = NavigationConstants.MAIN_NAV_MODS_GUID,
                    Parameter = modFiles
                });
        });
    }
}
