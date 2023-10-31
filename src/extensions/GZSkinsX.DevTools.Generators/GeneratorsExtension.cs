// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Contracts.Controls;
using GZSkinsX.Contracts.Extension;
using GZSkinsX.Contracts.Helpers;

namespace GZSkinsX.DevTools.Generators;

[Shared, ExtensionContract]
internal sealed class GeneratorsExtension : IExtension
{
    public ExtensionConfiguration ExtensionConfiguration => new()
    {
        Metadata = new()
        {
            Id = "DevTools.Generators.c18c4e5b-6000-488b-b9b4-a2921f4bf24c",
            Version = typeof(GeneratorsExtension).Assembly.GetAssemblyVersion(),
            Desctiption = ResourceHelper.GetLocalized("GZSkinsX.DevTools.Generators.x/Resources/ExtensionMetadata_Description"),
            DisplayName = "Generators Extension For GZSkinsX App",
            PublisherName = "Wenveo@outlook.com",
            Icon = new SegoeFluentIcon("\uE945")
        }
    };
}
