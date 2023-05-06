// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Api.ContextMenu;

public abstract class ContextToggleMenuItemBase<TContext> : IContextToggleMenuItem where TContext : IContextMenuUIContext
{
    public string? Header { get; protected set; }

    public IconElement? Icon { get; protected set; }

    public ContextMenuItemHotKey? HotKey { get; protected set; }

    public virtual bool IsChecked(TContext context) => false;

    public virtual bool IsEnabled(TContext context) => true;

    public virtual bool IsVisible(TContext context) => true;

    public abstract void OnExecute(TContext context);

    public abstract void OnToggle(bool newValue, TContext context);

    void IContextMenuItem.OnExecute(IContextMenuUIContext ctx) => OnExecute((TContext)ctx);

    bool IContextToggleMenuItem.IsChecked(IContextMenuUIContext context) => IsChecked((TContext)context);

    bool IContextMenuItem.IsEnabled(IContextMenuUIContext ctx) => IsEnabled((TContext)ctx);

    bool IContextMenuItem.IsVisible(IContextMenuUIContext ctx) => IsVisible((TContext)ctx);

    void IContextToggleMenuItem.OnToggle(bool newValue, IContextMenuUIContext context) => OnToggle(newValue, (TContext)context);
}
