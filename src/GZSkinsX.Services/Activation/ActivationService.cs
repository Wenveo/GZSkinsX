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

using Windows.ApplicationModel.Activation;

namespace GZSkinsX.Services.Activation;

[Shared, Export(typeof(IActivationService))]
internal sealed class ActivationService : IActivationService
{
    private readonly List<IActivationHandler> _handlers = new();

    public async Task ActivateAsync(IActivatedEventArgs args)
    {
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
