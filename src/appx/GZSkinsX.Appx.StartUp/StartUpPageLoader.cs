// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Api;
using GZSkinsX.Api.Shell;
using GZSkinsX.Appx.StartUp;

using Windows.UI.Xaml.Controls;

[Shared, ExportViewElement]
[ViewElementMetadata(Guid = ViewElementConstants.StartUpPage_Guid, PageType = typeof(StartUpPage))]
internal sealed class StartUpPageLoader : IViewElementLoader
{
    public void OnNavigating(WindowFrameNavigateEventArgs args)
    {
    }

    public void OnInitialize(Page viewElement)
    {
    }
}
