// Copyright 2022 - 2023 GZSkins, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace GZSkinsX.Contracts.Extension;

/// <summary>
/// 应用程序扩展接口，所有的 "应用程序扩展" 都继承于此
/// </summary>
public interface IExtension
{
    /// <summary>
    /// 应用程序扩展的名称
    /// </summary>
    string? Name { get; set; }

    /// <summary>
    /// 应用程序扩展的简短描述
    /// </summary>
    string? ShortDescription { get; set; }

    /// <summary>
    /// 应用程序扩展的版权信息
    /// </summary>
    string? Copyright { get; set; }

    /// <summary>
    /// 用于合并的资源字典列表，资源路径必须是相对于程序集的
    /// </summary>
    IEnumerable<string> MergedResourceDictionaries { get; }

    /// <summary>
    /// 应用程序扩展事件
    /// </summary>
    /// <param name="e">触发的事件类型</param>
    void OnEvent(ExtensionEvent e);
}
