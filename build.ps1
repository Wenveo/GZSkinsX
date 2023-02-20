[CmdletBinding(PositionalBinding=$false)]
Param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration,
    [ValidateSet("x86", "x64", "ARM64")]
    [string]$Platform
)

function Restore-Solution {
    msbuild $SolutionName /t:restore /p:Configuration=$Configuration /p:Platform=$Platform
    if ($LastExitCode -ne 0) {
        Write-Error "Failed to restore the solution."
        exit 1
    }
}

function Build-Extensions {
    $ExtensionDirectory = "src\extensions"
    $ExtensionProjects = Get-ChildItem -Path $ExtensionDirectory -Include *.csproj -Force -Recurse
    foreach ($proj in $ExtensionProjects) {
        msbuild $proj /p:Configuration=$Configuration /p:Platform=$Platform
        if ($LastExitCode -ne 0) {
            Write-Error "Failed to build the extension project: "$proj""
            exit 1
        }
    }
}

function Build-MainAppx {
    $SolutionName = "GZSkinsX.sln"
    $MainAppxProject = "src\appx\GZSkinsX\GZSkinsX.csproj"
    msbuild $MainAppxProject $MSBuildProperties /p:SelfContained=false /p:WindowsAppSDKSelfContained=false /p:GenerateTemporaryStoreCertificate=false
    if ($LastExitCode -ne 0) {
        Write-Error "Failed to build the main appx project."
        exit 1
    }
}

Restore-Solution
Build-Extensions
Build-MainAppx