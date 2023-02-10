using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using Windows.ApplicationModel;

namespace GZSkinsX.Contract.App;

public static class AppxContext
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetCurrentPackageFullName(
        ref int packageFullNameLength,
        StringBuilder? packageFullName);

    static AppxContext()
    {
        var length = 0;

        IsMSIX = GetCurrentPackageFullName(ref length, null) != 15700L;
        if (IsMSIX)
        {
            AppxDirectory = Package.Current.InstalledLocation.Path;

            var packageVersion = Package.Current.Id.Version;
            AppxVersion = new Version(packageVersion.Major, packageVersion.Minor,
                packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            AppxDirectory = Environment.CurrentDirectory;
            AppxVersion = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(-1, -1);
        }
    }

    public static bool IsMSIX { get; }

    public static string AppxDirectory { get; }

    public static Version AppxVersion { get; }
}
