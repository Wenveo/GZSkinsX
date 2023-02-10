// Licensed to the GZSkins, Inc. under one or more agreements.
// The GZSkins, Inc. licenses this file to you under the MS-PL license.

using System.Composition;
using System.Diagnostics;

using GZSkinsX.Contracts.App;

namespace GZSkinsX.MainApp;

[Shared]
[Export, Export(typeof(IAppWindow))]
internal sealed class AppWindow : IAppWindow
{
    public void Log(string message) => Debug.WriteLine(message);
}
