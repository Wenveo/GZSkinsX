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

using System.Reflection;

using GZSkinsX.Composition;
using GZSkinsX.Contracts.App;
using GZSkinsX.MainApp;

using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GZSkinsX;
/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App() => InitializeComponent();

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        var catalog = new AssemablyCatalogV2();
        var unused = catalog
            .AddParts(GetAssemblies())
            .AddParts(GetExtensionAssemblies());

        var container = new CompositionContainerV2(catalog);
        var exportProvider = await container.CreateExportProviderAsync(true);

        new MainWindow().Activate();
    }

    private static IEnumerable<Assembly> GetAssemblies()
    {
        yield return typeof(AppWindow).Assembly;
        yield return typeof(IAppWindow).Assembly;
    }

    private static IEnumerable<Assembly> GetExtensionAssemblies()
    {
        var root = new DirectoryInfo(AppxContext.AppxDirectory);
        foreach (var appx in root.GetFiles("GZSkinsX.Appx*", SearchOption.TopDirectoryOnly))
        {
            yield return Assembly.LoadFrom(appx.FullName);
        }

        foreach (var exAsm in root.GetFiles("*.x.dll", SearchOption.AllDirectories))
        {
            yield return Assembly.LoadFrom(exAsm.FullName);
        }
    }
}
