// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;

using GZSkinsX.SDK.CreatorStudio.Documents;

using Windows.Foundation;

namespace GZSkinsX.Extensions.CreatorStudio.Documents;

[Shared, Export(typeof(IDocumentService))]
internal sealed class DocumentService : IDocumentService
{
    private readonly IDocumentProviderService _documentProviderService;
    private readonly List<IDocument> _documents;
    private readonly ReaderWriterLockSlim _lock;

    public event TypedEventHandler<IDocumentService, DocumentCollectionChangedEventArgs>? CollectionChanged;

    [ImportingConstructor]
    public DocumentService(IDocumentProviderService documentProviderService)
    {
        _documentProviderService = documentProviderService;
        _documents = new();
        _lock = new();
    }

    public IDocument? CreateDocument(DocumentInfo documentInfo)
    {
        if (documentInfo is null)
        {
            throw new ArgumentNullException(nameof(documentInfo));
        }

        var provider = _documentProviderService.GetProvider(documentInfo.TypedGuid);
        if (provider is not null)
        {
            return provider.CreateDocument(documentInfo);
        }

        return null;
    }

    public IDocumentKey? CreateDocumentKey(DocumentInfo documentInfo)
    {
        if (documentInfo is null)
        {
            throw new ArgumentNullException(nameof(documentInfo));
        }

        var provider = _documentProviderService.GetProvider(documentInfo.TypedGuid);
        if (provider is not null)
        {
            return provider.CreateKey(documentInfo);
        }

        return null;
    }

    public IDocument[] GetDocuments()
    {
        _lock.EnterReadLock();
        IDocument[] documents;
        try
        {
            documents = _documents.ToArray();
        }
        finally
        {
            _lock.ExitReadLock();
        }

        return documents;
    }

    public int GetDocumentCount()
    {
        _lock.EnterReadLock();
        int documentCount;
        try
        {
            documentCount = _documents.Count;
        }
        finally
        {
            _lock.ExitReadLock();
        }

        return documentCount;
    }

    public void Clear()
    {
        _lock.EnterWriteLock();
        IDocument[] removedDocuments;
        try
        {
            removedDocuments = _documents.ToArray<IDocument>();
            _documents.Clear();
        }
        finally
        {
            _lock.ExitWriteLock();
        }

        if (removedDocuments.Length != 0)
        {
            CollectionChanged?.Invoke(this, new(DocumentCollectionEventType.Clear, removedDocuments));
        }
    }

    public IDocument? Find(IDocumentKey key)
    {
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        _lock.EnterReadLock();
        IDocument? document;
        try
        {
            document = FindCore(key);
        }
        finally
        {
            _lock.ExitReadLock();
        }

        if (document is not null)
        {
            return document;
        }

        return null;
    }

    private IDocument? FindCore(IDocumentKey key)
    {
        for (var i = 0; i < _documents.Count; i++)
        {
            var item = _documents[i];
            if (key.Equals(item.Key))
            {
                return item;
            }
        }

        return null;
    }

    public IDocument? GetOrAdd(IDocument? document)
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        _lock.EnterUpgradeableReadLock();
        IDocument? retDocument;
        try
        {
            var document2 = FindCore(document.Key);
            if (document2 is not null)
            {
                retDocument = document2;
            }
            else
            {
                _lock.EnterWriteLock();
                try
                {
                    _documents.Add(document);
                    retDocument = document;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        finally
        {
            _lock.ExitUpgradeableReadLock();
        }

        CollectionChanged?.Invoke(this, new(DocumentCollectionEventType.Add, new[] { retDocument }));
        return retDocument;
    }

    public void Remove(IDocumentKey key)
    {
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        _lock.EnterWriteLock();
        IDocument? document;
        try
        {
            document = RemoveCore(key);
        }
        finally
        {
            _lock.ExitWriteLock();
        }

        if (document is not null)
        {
            CollectionChanged?.Invoke(this, new(DocumentCollectionEventType.Remove, new[] { document }));
        }
    }

    private IDocument? RemoveCore(IDocumentKey key)
    {
        for (var i = 0; i < _documents.Count; i++)
        {
            var document = _documents[i];
            if (key.Equals(document.Key))
            {
                _documents.RemoveAt(i);
                return document;
            }
        }

        return null;
    }

    public void Remove(IEnumerable<IDocumentKey> keys)
    {
        if (keys is null)
        {
            throw new ArgumentNullException(nameof(keys));
        }

        var removedDocuments = new List<IDocument>();
        _lock.EnterWriteLock();
        try
        {
            for (var i = 0; i < _documents.Count; i++)
            {
                var document = _documents[i];
                foreach (var item in keys)
                {
                    if (item.Equals(document.Key))
                    {
                        _documents.RemoveAt(i--);
                        removedDocuments.Add(document);

                        break;
                    }
                }
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }

        if (removedDocuments.Count > 0)
        {
            CollectionChanged?.Invoke(this, new(DocumentCollectionEventType.Remove, removedDocuments.ToArray()));
        }
    }

    public void Remove(IEnumerable<IDocument> documents)
    {
        if (documents is null)
        {
            throw new ArgumentNullException(nameof(documents));
        }

        var removedDocuments = new List<IDocument>();
        _lock.EnterWriteLock();
        try
        {
            for (var i = 0; i < _documents.Count; i++)
            {
                var document = _documents[i];
                foreach (var item in documents)
                {
                    if (item.Key.Equals(document.Key))
                    {
                        _documents.RemoveAt(i--);
                        removedDocuments.Add(document);

                        break;
                    }
                }
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }

        if (removedDocuments.Count > 0)
        {
            CollectionChanged?.Invoke(this, new(DocumentCollectionEventType.Remove, removedDocuments.ToArray()));
        }
    }
}
