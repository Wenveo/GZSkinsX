// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;

namespace GZSkinsX.Appx.Backdrops;

/// <summary>
/// 提供将窗口背景设置为云母材质的能力，该功能仅可在 Windows 11 系统上可用 
/// </summary>
internal sealed class MicaSystemBackdrop : IDisposable
{
    /// <inheritdoc cref="WindowBackdropManager._target"/>
    private readonly ICompositionSupportsSystemBackdrop _target;

    /// <inheritdoc cref="WindowBackdropManager._configurationSource"/>
    private readonly SystemBackdropConfiguration _configuration;

    /// <summary>
    /// 云母材质控制器，可对目标对象设置或清除云母材质背景
    /// </summary>
    private readonly MicaController? _micaController;

    /// <summary>
    /// 用于判断当前类是否调用过 Dispose
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// 初始化 <see cref="MicaSystemBackdrop"/> 的新实例 
    /// </summary>
    /// <param name="target">材质目标对象</param>
    /// <param name="configuration">配置源，用于更新窗口输入焦点</param>
    public MicaSystemBackdrop(
        ICompositionSupportsSystemBackdrop target,
        SystemBackdropConfiguration configuration)
    {
        _target = target;
        _configuration = configuration;

        if (MicaController.IsSupported())
        {
            _micaController = new MicaController();
        }
    }

    /// <summary>
    /// 对当前目标对象应用云母材质背景
    /// </summary>
    /// <param name="useMicaAlt">当传入 true 时表示使用 MicaAlt，反之为 Mica</param>
    /// <returns>如果设置成功则返回 true，反之为 false</returns>
    public bool Apply(bool useMicaAlt)
    {
        if (_micaController is not null)
        {
            _configuration.IsInputActive = true;

            _micaController.Kind = useMicaAlt ? MicaKind.BaseAlt : MicaKind.Base;
            _micaController.AddSystemBackdropTarget(_target);
            _micaController.SetSystemBackdropConfiguration(_configuration);

            return true;
        }

        return false;
    }

    /// <summary>
    /// 对当前目标对象移除云母材质背景，并重置当前材质控制器的属性
    /// </summary>
    public void CleanUp()
    {
        if (_micaController is not null)
        {
            _micaController.RemoveSystemBackdropTarget(_target);
            _micaController.ResetProperties();
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            _micaController?.Dispose();
            _disposed = true;
        }
    }
}
