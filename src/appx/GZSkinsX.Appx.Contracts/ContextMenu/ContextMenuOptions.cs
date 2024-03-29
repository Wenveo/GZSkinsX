// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace GZSkinsX.Contracts.ContextMenu;

/// <inheritdoc cref="IContextMenuOptions"/>
public sealed class ContextMenuOptions : IContextMenuOptions
{
    /// <inheritdoc/>
    public bool AllowFocusOnInteraction { get; init; }

    /// <inheritdoc/>
    public bool AllowFocusWhenDisabled { get; init; }

    /// <inheritdoc/>
    public bool AreOpenCloseAnimationsEnabled { get; init; }

    /// <inheritdoc/>
    public LightDismissOverlayMode LightDismissOverlayMode { get; init; }

    /// <inheritdoc/>
    public DependencyObject? OverlayInputPassThroughElement { get; init; }

    /// <inheritdoc/>
    public FlyoutPlacementMode Placement { get; init; }

    /// <inheritdoc/>
    public bool ShouldConstrainToRootBounds { get; init; }

    /// <inheritdoc/>
    public FlyoutShowMode ShowMode { get; init; }

    /// <summary>
    /// 初始化 <see cref="ContextMenuOptions"/> 的新实例。
    /// </summary>
    public ContextMenuOptions()
    {
        AllowFocusOnInteraction = true;
        AreOpenCloseAnimationsEnabled = true;
        LightDismissOverlayMode = LightDismissOverlayMode.Auto;
        Placement = FlyoutPlacementMode.Top;
        ShowMode = FlyoutShowMode.Standard;
    }
}
