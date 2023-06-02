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

namespace GZSkinsX.SDK.CreatorStudio.Documents.Tabs;

public interface IDocumentTabService
{
    IDocumentTab? ActiveTab { get; }

    IEnumerable<IDocumentTab> DocumentTabs { get; }

    event EventHandler<ActiveDocumentTabChangedEventArgs>? ActiveTabChanged;

    event TypedEventHandler<IDocumentTabService, DocumentTabCollectionChangedEventArgs>? CollectionChanged;

    void Close(IDocumentTab tab);

    void CloseActiveTab();

    void CloseAllButThis(IDocumentTab tab);

    void CloseAllTabs();

    void SetActiveTab(int index);

    void SetActiveTab(IDocumentTab tab);
}
