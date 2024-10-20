if(Test-Path -LiteralPath "$PSScriptRoot\src\bin\"){
    Remove-Item -LiteralPath "$PSScriptRoot\src\bin\" -Force -Recurse | Out-Null
}

$DotNetCMD = Get-Command dotnet.exe -CommandType Application
$Solution = "$PSScriptRoot\src\FMFlattenFolders.sln"
$Params1 = 'build', '--configuration', 'Release', $Solution
$Params2 = 'build', '--configuration', 'Debug', $Solution

& $DotNetCMD $Params1
& $DotNetCMD $Params2

$BuildRelease = "$PSScriptRoot\src\bin\Release\net8.0"
$BuildDebug = "$PSScriptRoot\src\bin\Debug\net8.0"

explorer.exe $BuildRelease

