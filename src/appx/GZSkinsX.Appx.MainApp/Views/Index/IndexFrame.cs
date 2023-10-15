// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using GZSkinsX.Contracts.WindowManager;

namespace GZSkinsX.Appx.MainApp.Views;

[System.Composition.Shared, WindowFrameContract(Guid = WindowFrameConstants.Index_Guid, PageType = typeof(IndexPage))]
internal sealed class IndexFrame : IWindowFrame
{
    /// <inheritdoc/>
    public bool CanNavigateTo(WindowFrameNavigatingEvnetArgs args)
    {
        return true;
    }
}
