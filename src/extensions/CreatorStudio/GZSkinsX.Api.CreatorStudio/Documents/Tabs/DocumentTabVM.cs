// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Api.CreatorStudio.Documents.Tabs;

public abstract class DocumentTabVM : ObservableObject, IDocumentTab
{
    public abstract IDocument Document { get; }

    public virtual IDocumentTabContent? Content
    {
        get => _content;
        protected set => SetProperty(ref _content, value);
    }

    public virtual IconSource? IconSource
    {
        get => _iconSource;
        protected set => SetProperty(ref _iconSource, value);
    }

    public virtual string? Title
    {
        get => _title;
        protected set => SetProperty(ref _title, value);
    }

    public virtual object? ToolTip
    {
        get => _toolTip;
        protected set => SetProperty(ref _toolTip, value);
    }

    public virtual void OnAdded() { }

    public virtual void OnCloseRequested(DocumentTabCloseRequestedEventArgs args) { }

    public virtual void OnRemoved() { }

    protected IDocumentTabContent? _content;
    protected IconSource? _iconSource;
    protected string? _title;
    protected object? _toolTip;
}

