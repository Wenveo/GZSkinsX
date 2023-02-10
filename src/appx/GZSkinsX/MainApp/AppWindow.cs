using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GZSkinsX.Contract.App;

namespace GZSkinsX.MainApp;

[Shared]
[Export, Export(typeof(IAppWindow))]
internal sealed class AppWindow : IAppWindow
{
}
