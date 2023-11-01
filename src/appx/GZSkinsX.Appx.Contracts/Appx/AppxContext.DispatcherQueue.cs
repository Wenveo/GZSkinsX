// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using System.Threading;

using Microsoft.UI.Dispatching;

[assembly: InternalsVisibleTo("GZSkinsX.App, PublicKey=00240000048000009400000006020000002400005253413100040000010001006d13ff08810f5c0937e5d36923b65fcd450723277974d8777efe5f5d919a78e8c9030d11f54f6044d3ecb2c1d94ea0ede3fb339774d048fbb9449e7f773ab757d5fd3c42351753d3ab19b8b68286ef28fd131441db8851cdf5d9853de411bfd62b08c260b13f8c65c8b1eb3df5c4fef891ec959dce72595cd109202cdbdfde9b")]

namespace GZSkinsX.Contracts.Appx;

public static partial class AppxContext
{
    /// <summary>
    /// 定义和暂存已获取的调度器队列对象实例。
    /// </summary>
    private static DispatcherQueue? s_dispatcherQueue;

    /// <summary>
    /// 获取当前应用上下文中的调度器队列对象实例。
    /// </summary>
    public static DispatcherQueue DispatcherQueue
    {
        get
        {
            if (s_dispatcherQueue is not null)
            {
                return s_dispatcherQueue;
            }

            if (SynchronizationContext.Current is DispatcherQueueSynchronizationContextX ctx)
            {
                s_dispatcherQueue = ctx.DispatcherQueue;
                return s_dispatcherQueue;
            }

            if (_resolver is not null)
            {
                s_dispatcherQueue = AppxWindow.MainWindow.DispatcherQueue;
                return s_dispatcherQueue;
            }

            throw new InvalidOperationException("Could not get the DispatcherQueue instance.");
        }
    }
}

internal sealed class DispatcherQueueSynchronizationContextX(DispatcherQueue dispatcherQueue) : SynchronizationContext
{
    public DispatcherQueue DispatcherQueue => dispatcherQueue;

    /// <inheritdoc/>
    public override void Post(SendOrPostCallback d, object? state)
    {
        ArgumentNullException.ThrowIfNull(d);
        dispatcherQueue.TryEnqueue(delegate
        {
            try
            {
                d(state);
            }
            catch (Exception ex)
            {
                WinRT.ExceptionHelpers.ReportUnhandledError(ex);
            }
        });
    }

    /// <inheritdoc/>
    public override void Send(SendOrPostCallback d, object? state)
    {
        throw new NotSupportedException("The send method is not supported, use Post instead.");
    }

    /// <inheritdoc/>
    public override SynchronizationContext CreateCopy()
    {
        return new DispatcherQueueSynchronizationContext(dispatcherQueue);
    }
}
