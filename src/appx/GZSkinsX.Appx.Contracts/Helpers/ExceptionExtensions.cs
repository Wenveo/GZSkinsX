// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Contracts.Helpers;

public static class ExceptionExtensions
{
    public static Task ShowErrorDialogAsync(this Exception excp)
    {
        return ShowErrorDialogAsync(excp, null);
    }

    public static async Task ShowErrorDialogAsync(this Exception excp, string? title)
    {
        await new ContentDialog
        {
            Content = new ScrollViewer
            {
                Content = new TextBlock
                {
                    Text =
                    $"""
                    {excp}: "{excp.Message}"

                        {excp.StackTrace}
                    """,
                    TextWrapping = TextWrapping.Wrap
                }
            },

            DefaultButton = ContentDialogButton.Close,
            Title = title ?? ResourceHelper.GetLocalized("GZSkinsX.Appx.Contracts/Resources/Common_ErrorDialog_Title"),
            CloseButtonText = ResourceHelper.GetLocalized("GZSkinsX.Appx.Contracts/Resources/Common_ErrorDialog_CloseButtonText"),
            XamlRoot = AppxContext.AppxWindow.MainWindow.Content.XamlRoot
        }.ShowAsync();
    }
}
