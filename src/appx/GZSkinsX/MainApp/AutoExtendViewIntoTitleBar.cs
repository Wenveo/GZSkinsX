// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Extension;

using Windows.UI;

namespace GZSkinsX.MainApp;

[Shared, ExportAutoLoaded]
[AutoLoadedMetadata(LoadType = AutoLoadedType.AppLoaded)]
internal sealed class AutoExtendViewIntoTitleBar : IAutoLoaded
{
    [ImportingConstructor]
    public AutoExtendViewIntoTitleBar(IAppxTitleBar appxTitleBar, IAppxTitleBarButton appxTitleBarButton)
    {
        appxTitleBar.ExtendViewIntoTitleBar = true;
        appxTitleBarButton.ButtonBackgroundColor = Colors.Transparent;
        appxTitleBarButton.ButtonInactiveBackgroundColor = Colors.Transparent;
    }
}
