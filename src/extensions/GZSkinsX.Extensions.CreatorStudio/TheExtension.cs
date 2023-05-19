// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Composition;
using System.Reflection;

using GZSkinsX.Api.Extension;

namespace GZSkinsX.Extensions.CreatorStudio;

[Shared, ExportUniversalExtension, UniversalExtensionMetadata]
public class TheExtension : IUniversalExtension
{
    public string Name => "Creator Studio";

    public string ShortDescription => "The extension of 'Creator Studio' for 'GZSkinsX' app.";

    public string Copyright => "Copyright ©  2022 - 2023";

    public IEnumerable<string> MergedResourceDictionaries
    {
        get
        {
            //yield return "Styles/TabViewFluent.xaml";
            yield break;
        }
    }

    public void OnEvent(UniversalExtensionEvent e)
    {

    }

    public static IEnumerable<Assembly> GetSubModules()
    {
        yield return typeof(GZSkinsX.Extensions.CreatorStudio.Text.SubModule).Assembly;
        yield break;
    }
}
