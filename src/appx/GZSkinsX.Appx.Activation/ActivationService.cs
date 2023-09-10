// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Composition;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Activation;
using GZSkinsX.Contracts.Appx;

using Microsoft.Windows.AppLifecycle;

using Windows.ApplicationModel.Activation;

namespace GZSkinsX.Appx.Activation;

[Shared, Export(typeof(IActivationService))]
internal sealed class ActivationService : IActivationService
{
    private readonly List<IActivationHandler> _handlers = new();

    public async Task ActivateAsync(AppActivationArguments args)
    {
        if (args.Data is IActivatedEventArgs uwpArgs)
        {
            AppxContext.LoggingService.LogAlways(
                "GZSkinsX.Appx.ActivationService.ActivateAsync",
                $"{{Type = {uwpArgs.GetType()}, Kind = {uwpArgs.Kind}, PreviousExecutionState = {uwpArgs.PreviousExecutionState}}}");
        }
        else
        {
            AppxContext.LoggingService.LogAlways(
                "GZSkinsX.Appx.ActivationService.ActivateAsync",
                $"{{Type = {args.Data.GetType()}, Kind = {args.Kind}}}");
        }

        for (var i = _handlers.Count - 1; i >= 0; i--)
        {
            var handler = _handlers[i];
            if (handler is IActivationHandler2 canHandleAsync)
            {
                if (await canHandleAsync.CanHandleAsync(args) is false)
                {
                    continue;
                }
            }
            else
            {
                if (handler.CanHandle(args) is false)
                {
                    continue;
                }
            }

            await handler.HandleAsync(args);
            break;
        }
    }

    public void RegisterHandler(IActivationHandler handler)
    {
        if (handler is null)
        {
            return;
        }

        _handlers.Add(handler);
    }

    public void UnregisterHandler(IActivationHandler handler)
    {
        if (handler is null)
        {
            return;
        }

        _handlers.Remove(handler);
    }
}
