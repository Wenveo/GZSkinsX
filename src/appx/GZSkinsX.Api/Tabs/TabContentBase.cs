// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Threading.Tasks;

using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Api.Tabs;

public abstract class TabContentBase : ITabContent
{
    public string? Title { get; protected set; }

    public IconSource? IconSource { get; protected set; }

    public object? ToolTip { get; protected set; }

    public object? UIObject { get; protected set; }

    public virtual void OnAdded() { }

    public virtual void OnCloseRequested(TabContentCloseRequestedEventArgs args) { }

    public virtual void OnInitialize() { }

    public virtual async Task OnInitializeAsync() { await Task.CompletedTask; }

    public virtual void OnRemoved() { }
}
