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

namespace GZSkinsX.Api.CreatorStudio.DocumentTabs;

public interface IDocumentTabService
{
    IDocumentTabContent? ActiveTab { get; }

    IEnumerable<IDocumentTabContent> AllDocumentTabs { get; }

    event EventHandler<ActiveDocumentTabChangedEventArgs>? ActiveTabChanged;

    event TypedEventHandler<IDocumentTabService, DocumentTabCollectionChangedEventArgs>? CollectionChanged;

    void Close(IDocumentTabContent documentTab);

    void CloseActiveTab();

    void CloseAllButActiveTab();

    void SetActiveTab(int index);

    void SetActiveTab(IDocumentTabContent documentTab);
}
