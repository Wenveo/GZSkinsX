// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.ContextMenu;
using GZSkinsX.Contracts.Navigation;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GZSkinsX.Appx.MainApp.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class CustomizedNavView : NavigationView, INavigationViewCustomSearchBox
{
    public AutoSuggestBox SearchBoxControl => MainSearchBoxControl;

    public string? DefaultPlaceholderText => null;

    public CustomizedNavView()
    {
        InitializeComponent();

        MainGlobalMenu.Flyout = AppxContext.ContextMenuService.CreateContextMenu(ContextMenuConstants.MAIN_GLOBALMENU_CTX_GUID,
            new ContextMenuOptions { Placement = Microsoft.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Bottom },
            (s, e) => new MainGlobalMenuUIContext(s, AppxContext.ThemeService.CurrentTheme));
    }

    public async Task InitializeAsync()
    {
        if (AppxContext.MotClientService.TryGetMotClientAgentWorkingDirectory(out _) is false)
        {
            MainLaunchButton.UpdateCompleted += OnMainLaunchButtonUpdateCompleted;
            VisualStateManager.GoToState(this, "DisableRootContent", true);
            await MainLaunchButton.OnUpdateAsync();
        }
        else if (await AppxContext.MotClientService.VerifyContentIntegrityAsync() is false)
        {
            await MainLaunchButton.OnUpdateAsync();
        }
        else
        {
            await MainLaunchButton.InitializeAsync();
        }
    }

    private void OnMainLaunchButtonUpdateCompleted(object? sender, System.EventArgs e)
    {
        MainLaunchButton.UpdateCompleted -= OnMainLaunchButtonUpdateCompleted;
        VisualStateManager.GoToState(this, "EnableRootContent", true);
    }
}
