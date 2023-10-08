// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Appx.Command;

/// <inheritdoc cref="ICommandBarItemContainer"/>
/// <typeparam name="T">指定 <see cref="UIObject"/> 成员的对象类型。</typeparam>
internal interface ICommandBarItemContainer<T> : ICommandBarItemContainer where T : ICommandBarElement
{
    /// <inheritdoc cref="ICommandBarItemContainer.UIObject"/>
    new T UIObject { get; }

    /// <inheritdoc cref="ICommandBarItemContainer.UIObject"/>
    ICommandBarElement ICommandBarItemContainer.UIObject => UIObject;
}
