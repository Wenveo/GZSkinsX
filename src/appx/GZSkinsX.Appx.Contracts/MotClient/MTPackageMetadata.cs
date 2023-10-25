// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.MotClient;

/// <summary>
/// 服务组件包的元数据信息结构。
/// </summary>
/// <param name="Author">作者名称。</param>
/// <param name="Version">版本字符串。</param>
/// <param name="Description">包描述。</param>
/// <param name="AboutTheAuthor">作者链接。</param>
/// <param name="SettingsFile">配置文件的路径（相对路径）。</param>
/// <param name="ExecutableFile">服务组件的启动程序（相对路径）。</param>
/// <param name="ProcStartupArgs">服务组件的启动参数。</param>
/// <param name="ProcTerminateArgs">终止服务组件的参数。</param>
/// <param name="OtherStartupArgs">其它的启动参数选项。</param>
public record class MTPackageMetadata(string Author, string Version, string Description, string AboutTheAuthor, string SettingsFile,
    string ExecutableFile, string ProcStartupArgs, string ProcTerminateArgs, MTPackageMetadataStartUpArgument[] OtherStartupArgs)
{
}
