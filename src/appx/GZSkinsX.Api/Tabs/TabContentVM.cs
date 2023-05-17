// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Api.Tabs;

public abstract class TabContentVM : ObservableObject, ITabContent
{
    public virtual string? Title
    {
        get => _title;
        protected set => SetProperty(ref _title, value);
    }

    public virtual IconSource? IconSource
    {
        get => _iconSource;
        protected set => SetProperty(ref _iconSource, value);
    }

    public virtual object? ToolTip
    {
        get => _toolTip;
        protected set => SetProperty(ref _toolTip, value);
    }

    public virtual object? UIObject
    {
        get => _uiObject;
        protected set => SetProperty(ref _uiObject, value);
    }

    public virtual void OnAdded() { }

    public virtual void OnCloseRequested(TabContentCloseRequestedEventArgs args) { }

    public virtual void OnInitialize() { }

    public virtual async Task OnInitializeAsync() { await Task.CompletedTask; }

    public virtual void OnRemoved() { }

    protected string? _title;
    protected IconSource? _iconSource;
    protected object? _toolTip;
    protected object? _uiObject;
}
