// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Api.Tabs;

public interface ITabContent
{
    string? Title { get; }

    IconSource? IconSource { get; }

    object? ToolTip { get; }

    object? UIObject { get; }

    void OnAdded();

    void OnCloseRequested(TabContentCloseRequestedEventArgs args);

    void OnRemoved();
}
