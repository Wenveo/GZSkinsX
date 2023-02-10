using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZSkinsX.Contract.Navigation;

/// <summary>
/// 被用于主视图中的导航服务
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// 获取当前 <see cref="INavigationService"/> 是否可以向后导航
    /// </summary>
    bool CanGoback { get; }

    /// <summary>
    /// 获取当前 <see cref="INavigationService"/> 是否可以向前导航
    /// </summary>
    bool CanGoForward { get; }

    /// <summary>
    /// 向后导航
    /// </summary>
    void GoBack();

    /// <summary>
    /// 向前导航
    /// </summary>
    void GoForward();
}
