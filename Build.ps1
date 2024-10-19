$DotNetCMD = Get-Command dotnet.exe -CommandType Application
$Solution = "$PSScriptRoot\src\FMFlattenFolders.sln"


$Params1 = 'build', '--configuration', 'Release', $Solution
$Params2 = 'build', '--configuration', 'Debug', $Solution

& $DotNetCMD $Params1
& $DotNetCMD $Params2

Read-Host
