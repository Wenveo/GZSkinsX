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

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace GZSkinsX.Contracts.App;

/// <summary>
/// 提供应用程序主窗口的事件，以及窗口管理相关的 Api
/// </summary>
public interface IAppWindow
{
    /// <summary>
    /// 当应用程序主窗口被激活时触发
    /// </summary>
    event EventHandler<WindowActivatedEventArgs>? Activated;

    /// <summary>
    /// 当应用程序主窗口被置为后台窗口时触发
    /// </summary>
    event EventHandler<WindowActivatedEventArgs>? Deactivated;

    /// <summary>
    /// 在应用程序主窗口关闭时触发
    /// </summary>
    event EventHandler<WindowEventArgs>? Closing;

    /// <summary>
    /// 在应用程序主窗口关闭之后触发
    /// </summary>
    event EventHandler? Closed;

    /// <summary>
    /// 当前应用程序主窗口
    /// </summary>
    Window MainWindow { get; }

    /// <summary>
    /// 激活当前应用程序主窗口
    /// </summary>
    void Activate();

    /// <summary>
    /// 将当前应用程序主窗口置于前台
    /// </summary>
    void BringToFront();

    /// <summary>
    /// 最小化当前应用程序主窗口
    /// </summary>
    void Minimize();

    /// <summary>
    /// 最大化当前应用程序主窗口
    /// </summary>
    void Maximize();

    /// <summary>
    /// 向下还原当前应用程序主窗口
    /// </summary>
    void Restore();

    /// <summary>
    /// 关闭当前应用程序主窗口
    /// </summary>
    void Close();

    /// <summary>
    /// 隐藏当前应用程序主窗口
    /// </summary>
    void Hide();

    /// <summary>
    /// 显示当前应用程序主窗口
    /// </summary>
    void Show();

    /// <summary>
    /// 设置当前应用程序主窗口的呈现类型
    /// </summary>
    /// <param name="kind">要呈现的类型</param>
    void SetWindowPresenter(AppWindowPresenterKind kind);
}
