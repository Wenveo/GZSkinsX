// Licensed to the GZSkins, Inc. under one or more agreements.
// The GZSkins, Inc. licenses this file to you under the MS-PL license.

using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using Windows.ApplicationModel;

namespace GZSkinsX.Contracts.App;

/// <summary>
/// 当前 Appx 应用的上下文
/// </summary>
public static class AppxContext
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetCurrentPackageFullName(
        ref int packageFullNameLength,
        StringBuilder? packageFullName);

    /// <summary>
    /// 初始化 <see cref="AppxContext"/> 的静态成员
    /// </summary>
    static AppxContext()
    {
        var length = 0;

        IsMSIX = GetCurrentPackageFullName(ref length, null) != 15700L;
        if (IsMSIX)
        {
            AppxDirectory = Package.Current.InstalledLocation.Path;

            var packageVersion = Package.Current.Id.Version;
            AppxVersion = new(packageVersion.Major, packageVersion.Minor,
                packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            AppxDirectory = Environment.CurrentDirectory;
            AppxVersion = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(-1, -1);
        }
    }

    /// <summary>
    /// 获取当前应用程序是否为 MSIX 应用
    /// </summary>
    public static bool IsMSIX { get; }

    /// <summary>
    /// 获取当前应用程序根目录
    /// <para>
    /// 根据 <see cref="IsMSIX"/> 的不同，返回的目录也不同
    /// </para>
    /// </summary>
    public static string AppxDirectory { get; }

    /// <summary>
    /// 获取当前应用程序版本
    /// </summary>
    public static Version AppxVersion { get; }
}
