using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GZSkinsX.Contract.Navigation;

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
