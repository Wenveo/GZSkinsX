// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System.Composition;
using System.Diagnostics;
using System.Reflection;

using GZSkinsX.Contract.App;
using GZSkinsX.Contract.Navigation;
using GZSkinsX.MainApp;

using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.Composition;

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
        this.InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        AppWindow = new MainWindow();
        AppWindow.Activate();
    }

    public Window? AppWindow { get; private set; }
}