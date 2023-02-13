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

using System.Composition;

using GZSkinsX.Contracts.App;
using GZSkinsX.Contracts.Extension;
using GZSkinsX.Extension;

using Microsoft.UI.Xaml;

namespace GZSkinsX.MainApp;

[Shared]
[Export, Export(typeof(IAppWindow))]
internal sealed class AppWindow : IAppWindow
{
    private readonly ExtensionService _extensionService;
    private readonly ShellWindow _shellWindow;

    public event EventHandler<WindowActivatedEventArgs>? Activated;

    public event EventHandler<WindowEventArgs>? Closed;

    public Window MainWindow => _shellWindow;

    [ImportingConstructor]
    public AppWindow(ExtensionService extensionService)
    {
        _extensionService = extensionService;
        _shellWindow = new ShellWindow();

        Activated += OnAppLoaded;
    }

    private void OnAppLoaded(object? sender, WindowActivatedEventArgs e)
    {
        Activated -= OnAppLoaded;
        _extensionService.NotifyExtensions(ExtensionEvent.AppLoaded);
    }

    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        Activated?.Invoke(this, args);
    }

    private void OnClosed(object sender, WindowEventArgs args)
    {
        Closed?.Invoke(this, args);

        if (!args.Handled)
        {
        _extensionService.NotifyExtensions(ExtensionEvent.AppExit);
    }
    }

    public void InitializeMainWindow()
    {
        _shellWindow.Activated += OnActivated;
        _shellWindow.Closed += OnClosed;
    }

    public void ShowWindow()
    {
        _shellWindow.Activate();
    }
}
