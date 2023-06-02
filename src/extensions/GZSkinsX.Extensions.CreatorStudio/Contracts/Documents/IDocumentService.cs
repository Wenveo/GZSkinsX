// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Collections.Generic;

using Windows.Foundation;

namespace GZSkinsX.Extensions.CreatorStudio.Contracts.Documents;

public interface IDocumentService
{
    event TypedEventHandler<IDocumentService, DocumentCollectionChangedEventArgs>? CollectionChanged;

    IDocumentKey? CreateDocumentKey(DocumentInfo documentInfo);

    IDocument? CreateDocument(DocumentInfo documentInfo);

    IDocument[] GetDocuments();

    int GetDocumentCount();

    void Clear();

    IDocument? Find(IDocumentKey key);

    IDocument? GetOrAdd(IDocument? document);

    void Remove(IDocumentKey key);

    void Remove(IEnumerable<IDocumentKey> keys);

    void Remove(IEnumerable<IDocument> documents);
}
