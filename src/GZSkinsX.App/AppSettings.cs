// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.ComponentModel;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.Settings;

using Windows.UI.Xaml;

namespace GZSkinsX;

[Shared, Export]
internal sealed class AppSettings
{
    private const string THE_GUID = "FF1E168C-21CB-4211-8422-03401AD881BA";
    private const string SHOULD_ENABLE_EFFICIENCY_MODE_NAME = "ShouldEnableEfficiencyMode";
    private const string DONT_NEED_CLOSE_WHEN_GAME_IS_LAUNCHED_NAME = "DontNeedCloseWhenGameIsLaunched";

    private readonly ISettingsSection _settingsSection;

    public bool ShouldEnableEfficiencyMode
    {
        get => _settingsSection.Attribute<bool>(SHOULD_ENABLE_EFFICIENCY_MODE_NAME);
        set
        {
            _settingsSection.Attribute(SHOULD_ENABLE_EFFICIENCY_MODE_NAME, value);
            App.DesktopExtensionMethods.SetEfficiencyMode(Process.GetCurrentProcess().Id, value).FireAndForget();
        }
    }

    public bool DontNeedCloseWhenGameIsLaunched
    {
        get => _settingsSection.Attribute<bool>(DONT_NEED_CLOSE_WHEN_GAME_IS_LAUNCHED_NAME);
        set => _settingsSection.Attribute(DONT_NEED_CLOSE_WHEN_GAME_IS_LAUNCHED_NAME, value);
    }

    public AppSettings()
    {
        _settingsSection = AppxContext.SettingsService.GetOrCreateSection(THE_GUID);
    }

    public void Initialize()
    {
        if (ShouldEnableEfficiencyMode)
        {
            App.DesktopExtensionMethods.SetEfficiencyMode(Process.GetCurrentProcess().Id, true);
        }

        if (DontNeedCloseWhenGameIsLaunched is false)
        {
            var bgWorker = new BackgroundWorker();
            bgWorker.DoWork += async (s, e) =>
            {
                bool b;
                do
                {
                    b = await App.DesktopExtensionMethods.IsProcessRunning("League of Legends");
                    await Task.Delay(300);
                }
                while (b is false);

                Application.Current.Exit();
            };

            bgWorker.RunWorkerAsync();
        }
    }
}
