// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Api.Appx;

namespace GZSkinsX.MainApp;

/// <see cref="IAppxTitleBar"/>
[Shared, Export(typeof(IAppxTitleBar))]
internal sealed class AppxTitleBar : IAppxTitleBar
{
    /// <inheritdoc/>
    public bool ExtendsContentIntoTitleBar
    {
        get => App.MainWindow.ExtendsContentIntoTitleBar;
        set => App.MainWindow.ExtendsContentIntoTitleBar = value;
    }

    /// <inheritdoc/>
    public void SetTitleBar(Microsoft.UI.Xaml.UIElement? value)
    => App.MainWindow.SetTitleBar(value);
}
