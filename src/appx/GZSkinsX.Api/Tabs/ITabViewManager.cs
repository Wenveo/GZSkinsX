// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;

using Windows.Foundation;

namespace GZSkinsX.Api.Tabs;

public interface ITabViewManager
{
    ITabContent? ActiveTab { get; }

    IEnumerable<ITabContent> TabContents { get; }

    object UIObject { get; }

    event EventHandler<ActiveTabChangedEventArgs>? ActiveTabChanged;

    event TypedEventHandler<ITabViewManager, TabCollectionChangedEventArgs>? CollectionChanged;

    void Add(ITabContent tabContent);

    void Close(ITabContent tabContent);

    void CloseActiveTab();

    void CloseAllButActiveTab();

    void SetActiveTab(int index);

    void SetActiveTab(ITabContent tabContent);
}
