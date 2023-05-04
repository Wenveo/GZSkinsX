// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Api.CreatorStudio.Commands;

public abstract class CommandToggleButtonBase : ICommandToggleButton
{
    public virtual string? GetDisplayName() => null;

    public virtual CommandHotKey? GetHotKey() => null;

    public virtual IconElement? GetIcon() => null;

    public virtual string? GetToolTip() => null;

    public virtual bool IsEnabled() => true;

    public virtual bool IsVisible() => true;

    public abstract void OnToggle(bool newValue, ICommandUIContext ctx);
}
