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
using System.Runtime.InteropServices;
using System.Text;

using Windows.ApplicationModel;

namespace GZSkinsX.Contracts.Appx;

/// <summary>
/// 当前 Appx 应用的上下文
/// </summary>
public static class AppxContext
{
    /// <summary>
    /// 获取当前应用包的名称
    /// </summary>
    /// <param name="packageFullNameLength">用于返回字符串长度</param>
    /// <param name="packageFullName">用于存储应用包名称</param>
    /// <returns></returns>
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
