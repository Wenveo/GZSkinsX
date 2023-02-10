// Licensed to the GZSkins, Inc. under one or more agreements.
// The GZSkins, Inc. licenses this file to you under the MS-PL license.

using System.Composition;

using GZSkinsX.Contracts.Navigation;

namespace GZSkinsX.Navigation;

[Shared]
[Export, Export(typeof(INavigationService))]
internal sealed class NavigationService : INavigationService
{
    public bool CanGoback => throw new NotImplementedException();

    public bool CanGoForward => throw new NotImplementedException();

    public void GoBack() => throw new NotImplementedException();

    public void GoForward() => throw new NotImplementedException();
}
