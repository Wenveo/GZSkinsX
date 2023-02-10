// Licensed to the GZSkins, Inc. under one or more agreements.
// The GZSkins, Inc. licenses this file to you under the MS-PL license.

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
    public App()
    {
        InitializeComponent();

        ThrowCatchTest();
    }

    static void ThrowCatchTest()
    {
        try
        {
            throw new ArgumentNullException("AAAA");
        }
        finally
        {

        }
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        var catalog = new AssemablyCatalogV2();
        catalog.AddParts(GetAssemblies());

        var container = new CompositionContainerV2(catalog);
        var exportProvider = await container.CreateExportProviderAsync(true);

        var appWindow = exportProvider.GetExportedValue<IAppWindow>();
        appWindow.Log("Test");

        AppWindow = new MainWindow();
        AppWindow.Activate();
    }


    public Window? AppWindow { get; private set; }

    private static IEnumerable<Assembly> GetAssemblies()
    {
        yield return typeof(AppWindow).Assembly;
        yield return typeof(IAppWindow).Assembly;
    }
}
