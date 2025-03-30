$Solution           = 'FMFlattenFolders.sln'
$CSProj             = 'FMFlattenFoldersCmdlet.csproj'
$Readme             = 'README.md'
$License            = 'LICENSE.md'
$BuildPath          = Join-Path $PSScriptRoot     -ChildPath 'bin'
$BuildDebugPath     = Join-Path $BuildPath        -ChildPath 'Debug' 'net8.0'
$BuildReleasePath   = Join-Path $BuildPath        -ChildPath 'Release' 'net8.0'
$ObjPath            = Join-Path $PSScriptRoot     -ChildPath 'obj'
$SolutionPath       = Join-Path $PSScriptRoot     -ChildPath $Solution
$ReadmePath         = Join-Path $PSScriptRoot     -ChildPath $Readme
$LicensePath        = Join-Path $PSScriptRoot     -ChildPath $License
$ReadmeDestDebug    = Join-Path $BuildDebugPath   -ChildPath $Readme
$ReadmeDestRelease  = Join-Path $BuildReleasePath -ChildPath $Readme
$LicenseDestDebug   = Join-Path $BuildDebugPath   -ChildPath $License
$LicenseDestRelease = Join-Path $BuildReleasePath -ChildPath $License
$NuGetAPIKey        = ($env:FM_NUGET_API_KEY -as [String]).Trim()

function Build-Module {
    Push-Location -LiteralPath $PSScriptRoot -StackName Build

    if(Test-Path -LiteralPath $BuildPath -PathType Container){
        Remove-Item -LiteralPath $BuildPath -Force -Recurse | Out-Null
    }
    if(Test-Path -LiteralPath $ObjPath -PathType Container){
        Remove-Item -LiteralPath $ObjPath -Force -Recurse | Out-Null
    }

    $cmdDotnet = Get-Command dotnet.exe -CommandType Application
    $null = & $cmdDotnet clean "$CSProj"
    $dotnetParams1 = @('add', 'package', 'PowerShellStandard.Library', '--version', '7.0.0-preview.1')
    $dotnetParams2 = @('add', 'package', 'System.Security.Permissions', '--version', '8.0.0')
    & $cmdDotnet $dotnetParams1
    & $cmdDotnet $dotnetParams2
    & $cmdDotnet build --configuration Release $SolutionPath
    & $cmdDotnet build --configuration Debug $SolutionPath
    $null = [IO.File]::Copy($ReadmePath, $ReadmeDestDebug)
    $null = [IO.File]::Copy($ReadmePath, $ReadmeDestRelease)
    $null = [IO.File]::Copy($LicensePath, $LicenseDestDebug)
    $null = [IO.File]::Copy($LicensePath, $LicenseDestRelease)
    Pop-Location -StackName Build
}

function Publish-ToPSGallery {
    if(Test-Path -LiteralPath $BuildReleasePath -PathType Container){
        Publish-PSResource -Path $BuildReleasePath -Repository PSGallery -APIKey $NuGetAPIKey -Verbose
    }
}

Build-Module
# Publish-ToPSGallery