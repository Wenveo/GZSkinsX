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

namespace GZSkinsX.Contracts.Navigation;

/// <summary>
/// 提供对主视图中的页面操作和管理的能力
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// 能否向后导航
    /// </summary>
    bool CanGoback { get; }

    /// <summary>
    /// 能否向前导航
    /// </summary>
    bool CanGoForward { get; }

    /// <summary>
    /// 向后导航
    /// </summary>
    void GoBack();

    /// <summary>
    /// 向前导航
    /// </summary>
    void GoForward();
}
