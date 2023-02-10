using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZSkinsX.Contract.Navigation;

/// <summary>
/// 导航菜单项，被用于 <see cref="INavigationService"/> 并在主视图中显示
/// </summary>
public interface INavigationViewItem
{
    /// <summary>
    /// 用于显示的名称
    /// </summary>
    string Header { get; }

    /// <summary>
    /// 默认显示的字形图标
    /// </summary>
    string Icon { get; }
}
