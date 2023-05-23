// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;

using GZSkinsX.Api.CreatorStudio.Documents;
using GZSkinsX.Api.CreatorStudio.Documents.Tabs;

namespace GZSkinsX.Extensions.CreatorStudio.Documents.Tabs;

[Shared, Export(typeof(IUndoManagerFactory))]
internal sealed class UndoManagerFactory : IUndoManagerFactory
{
    public IUndoManager Create(IDocumentKey key)
    {
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        return new UndoManager(key);
    }
}
