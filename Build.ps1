$ProjectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$SourceRoot = [System.IO.Path]::Combine($ProjectRoot, 'src')
$Solution = [System.IO.Path]::Combine($SourceRoot, 'FMFlattenFolders.sln')
$AutoBuildDir = [System.IO.Path]::Combine($SourceRoot, 'bin')
$FinalBuildDir = [System.IO.Path]::Combine($ProjectRoot, 'build')

if(Test-Path -LiteralPath $AutoBuildDir -PathType Container){
    Remove-Item -LiteralPath $AutoBuildDir -Force -Recurse | Out-Null
}

$DotNetCMD = Get-Command dotnet.exe -CommandType Application
& $DotNetCMD build --configuration Release $Solution

[IO.Directory]::Move($AutoBuildDir, $FinalBuildDir) | Out-Null


