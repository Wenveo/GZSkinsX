// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Api.ContextMenu;

public interface IContextMenuItem
{
    string? Header { get; }

    IconElement? Icon { get; }

    ContextMenuItemHotKey? HotKey { get; }

    bool IsVisible(IContextMenuUIContext context);

    bool IsEnabled(IContextMenuUIContext context);

    void OnExecute(IContextMenuUIContext context);
}
