// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;

namespace GZSkinsX.Api.CreatorStudio.DocumentTabs;

public sealed class DocumentTabCollectionChangedEventArgs : EventArgs
{
    public IDocumentTabContent[]? AddedItems { get; }

    public IDocumentTabContent[]? RemovedItems { get; }

    public DocumentTabCollectionChangedEventArgs(IDocumentTabContent[]? addedItems, IDocumentTabContent[]? removedItems)
    {
        AddedItems = addedItems;
        RemovedItems = removedItems;
    }
}
