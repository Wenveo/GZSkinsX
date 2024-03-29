// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.Extension;

/// <summary>
/// 应用程序的扩展类。
/// </summary>
public interface IExtension : IExtensionClass
{
    /// <summary>
    /// 获取此扩展的可选配置。
    /// </summary>
    ExtensionConfiguration ExtensionConfiguration { get; }

    /// <summary>
    /// 在应用加载完成后触发的事件方法。
    /// </summary>
    void OnAppLoaded() { }

    /// <summary>
    /// 在应用退出时触发的事件方法。
    /// </summary>
    void OnAppExit() { }
}
