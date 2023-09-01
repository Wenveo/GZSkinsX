// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Extension;

namespace GZSkinsX.Activation;

[Shared, ExportAdvanceExtension]
[AdvanceExtensionMetadata(Order = double.MinValue)]
internal sealed class AutoInitializeHandlers : IAdvanceExtension
{
    public AutoInitializeHandlers()
    {
        var activationService = AppxContext.ActivationService;
        activationService.RegisterHandler(new ModFileActivationHandler());
    }
}
