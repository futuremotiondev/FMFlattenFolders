if(Test-Path -LiteralPath "$PSScriptRoot\src\bin\"){
    Remove-Item -LiteralPath "$PSScriptRoot\src\bin\" -Force -Recurse | Out-Null
}

$DotNetCMD = Get-Command dotnet.exe -CommandType Application
$Solution = "$PSScriptRoot\src\FMFlattenFolders.sln"
$Params1 = 'build', '--configuration', 'Release', $Solution
$Params2 = 'build', '--configuration', 'Debug', $Solution

& $DotNetCMD $Params1 | Out-Null
& $DotNetCMD $Params2 | Out-Null

$BuildRelease = "$PSScriptRoot\src\bin\Release\net8.0"
$BuildDebug = "$PSScriptRoot\src\bin\Debug\net8.0"

$VSYSModuleDirectory = "D:\Dev\Powershell\VSYSModules\FMFlattenFolders"
if(Test-Path -LiteralPath $VSYSModuleDirectory){
    Remove-Item -LiteralPath $VSYSModuleDirectory -Force -Recurse | Out-Null
}
New-Item -Path $VSYSModuleDirectory -ItemType Directory -Force | Out-Null

Get-ChildItem -Path $BuildRelease -File | Copy-Item -Destination $VSYSModuleDirectory -Recurse -Force | Out-Null
Set-Location -LiteralPath $VSYSModuleDirectory

explorer.exe $VSYSModuleDirectory

Read-Host -Prompt "Press any key to exit."

