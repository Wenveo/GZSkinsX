// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace GZSkinsX.Api.CreatorStudio.Documents.Tabs;

public abstract class DocumentTabContentVM : ObservableObject, IDocumentTabContent, IDocumentTabContent2
{
    public virtual object? UIObject
    {
        get => _uiObject;
        protected set => SetProperty(ref _uiObject, value);
    }

    public virtual void OnInitialize() { }

    public virtual async Task OnInitializeAsync() { await Task.CompletedTask; }

    protected object? _uiObject;
}
