// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Api.Extension;

using Microsoft.UI.Xaml;

namespace GZSkinsX.MainApp;

[Shared, ExportAdvanceExtension]
[AdvanceExtensionMetadata(Trigger = AdvanceExtensionTrigger.AppLoaded)]
internal sealed class AutoMergeResources : IAdvanceExtension
{
    public AutoMergeResources()
    {
        var mergedResourceDictionaries = Application.Current.Resources.MergedDictionaries;
        foreach (var rsrc in StartUpClass.ExtensionService.GetMergedResourceDictionaries())
        {
            mergedResourceDictionaries.Add(rsrc);
        }
    }
}
