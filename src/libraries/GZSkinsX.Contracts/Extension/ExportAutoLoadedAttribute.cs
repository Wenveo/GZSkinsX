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

using System.Composition;

namespace GZSkinsX.Contracts.Extension;

/// <summary>
/// 声明并导出为自动加载的扩展
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ExportAutoLoadedAttribute : ExportAttribute
{
    /// <summary>
    /// 初始化 <see cref="ExportAutoLoadedAttribute"/> 的新实例，并以 <see cref="IAutoLoaded"/> 类型导出
    /// </summary>
    public ExportAutoLoadedAttribute()
        : base(typeof(IAutoLoaded)) { }
}
