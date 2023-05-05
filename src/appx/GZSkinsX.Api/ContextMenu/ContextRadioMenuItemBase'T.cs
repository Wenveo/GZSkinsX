// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Api.ContextMenu;

public abstract class ContextRadioMenuItemBase<TContext> : IContextRadioMenuItem where TContext : IContextMenuUIContext
{
    public virtual string? GetGroupName() => null;

    public virtual string? GetHeader(TContext ctx) => null;

    public virtual ContextMenuItemHotKey? GetHotKey(TContext context) => null;

    public virtual IconElement? GetIcon(TContext context) => null;

    public virtual bool IsChecked(TContext context) => false;

    public virtual bool IsEnabled(TContext context) => true;

    public virtual bool IsVisible(TContext context) => true;

    public abstract void OnClick(bool isChecked, TContext context);

    public abstract void OnExecute(TContext context);

    string? IContextRadioMenuItem.GetGroupName() => GetGroupName();

    string? IContextMenuItem.GetHeader(IContextMenuUIContext ctx) => GetHeader((TContext)ctx);

    ContextMenuItemHotKey? IContextMenuItem.GetHotKey(IContextMenuUIContext ctx) => GetHotKey((TContext)ctx);

    IconElement? IContextMenuItem.GetIcon(IContextMenuUIContext ctx) => GetIcon((TContext)ctx);

    bool IContextRadioMenuItem.IsChecked(IContextMenuUIContext context) => IsChecked((TContext)context);

    bool IContextMenuItem.IsEnabled(IContextMenuUIContext ctx) => IsEnabled((TContext)ctx);

    bool IContextMenuItem.IsVisible(IContextMenuUIContext ctx) => IsVisible((TContext)ctx);

    void IContextRadioMenuItem.OnClick(bool isChecked, IContextMenuUIContext context) => OnClick(isChecked, (TContext)context);

    void IContextMenuItem.OnExecute(IContextMenuUIContext ctx) => OnExecute((TContext)ctx);
}
