// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Contracts.Extension;

/// <summary>
/// 关于扩展的描述信息。
/// </summary>
public sealed record class ExtensionMetadata
{
    /// <summary>
    /// 获取扩展的唯一标识符。
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// 获取显示在扩展管理器 UI 中的图标，如果没有指定图标元素，UI 将使用默认值。
    /// </summary>
    public IconElement? Icon { get; init; }

    /// <summary>
    /// 获取扩展的版本。
    /// </summary>
    public string? Version { get; init; }

    /// <summary>
    /// 获取扩展的发布者名称。
    /// </summary>
    public string? PublisherName { get; init; }

    /// <summary>
    /// 获取显示在扩展管理器 UI 中的包及其内容的可选简短说明。
    /// </summary>
    public string? Desctiption { get; init; }

    /// <summary>
    /// 获取显示在扩展管理器 UI 中的包及其内容的可选简短说明。
    /// </summary>
    public string? DisplayName { get; init; }

    /// <summary>
    /// 初始化 <see cref="ExtensionMetadata"/> 的新实例。
    /// </summary>
    public ExtensionMetadata()
    {

    }

    /// <summary>
    /// 初始化 <see cref="ExtensionMetadata"/> 的新实例。
    /// </summary>
    /// <param name="id">扩展的唯一标识符。</param>
    /// <param name="version">扩展的版本。</param>
    /// <param name="publisherName">扩展的发布者名称。</param>
    /// <param name="description">用于显示在扩展管理器 UI 中的描述说明。</param>
    /// <param name="displayName">用于显示在扩展管理器 UI 中的用户友好的包名称。</param>
    public ExtensionMetadata(string? id, string? version, string? publisherName, string? description, string? displayName)
    {
        Id = id;
        Version = version;
        PublisherName = publisherName;
        Desctiption = description;
        DisplayName = displayName;
    }
}
