# FMFlattenFolders

This is a modified version of FlattenFolders by [Rob Green](https://github.com/trossr32/ps-flatten-folders). I've simply added an additional method of renaming duplicate files.

<img alt="PowerShell Gallery Version" src="https://img.shields.io/powershellgallery/v/FMFlattenFolders?link=https%3A%2F%2Fwww.powershellgallery.com%2Fpackages%2FFMFlattenFolders%2F2.0.2">
<img alt="GitHub last commit (branch)" src="https://img.shields.io/github/last-commit/futuremotiondev/FMFlattenFolders/main?link=https%3A%2F%2Fgithub.com%2Ffuturemotiondev%2FFMFlattenFolders&link=https%3A%2F%2Fgithub.com%2Ffuturemotiondev%2FFMFlattenFolders">




## Installation (from the Powershell Gallery)

```powershell
Install-Module FMFlattenFolders
Import-Module FMFlattenFolders
```

## Description

Moves files from all sub-directories to the parent directory. If files with duplicate names are found, they will have either a GUID or a numeric index appended to the file name depending on the passed value of `-RenameMethod`.

The cmdlet also Supports `-WhatIf`. If supplied this will output a formatted table of the final locations of all files.

## Usage

The below will completely flatten `C:\Music`, copying all files in all subdirectories recursively to the root.

```powershell
Invoke-FlattenFolders -Directory "C:\Music"
```

The below will completely flatten `D:\Icons\SVG`, copying all files in all subdirectories recursively to the root, and on completion delete all empty subdirectories after the move.

```powershell
Invoke-FlattenFolders -Directory "D:\Icons\SVG" -DeleteSubDirectories
```

You can also pipe multiple directories to flatten in an array:

```powershell
@("C:\MyVideos", "C:\ConvertedAudio", "D:\SomeOther\Directory") |
Invoke-FlattenFolders -DeleteSubDirectories
```

`-RenameMethod` determines how duplicate files are handled during the directory flattening operation.

`-RenameMethod Index` will append a numeric index to the end of duplicate files.

`-RenameMethod GUID` will append a random GUID to the end of duplicate files.

```powershell
Invoke-FlattenFolders -Directory "D:\Icons\SVG" -DeleteSubDirectories -RenameMethod Index
```

## Parameters

#### `-Directory` (alias -D)
*Optional*. The parent directory where files from all sub-directories will be moved. If neither this nor the Directories parameter are set then the current location will be used.

#### `-Directories`
*Optional*. A collection of parent directories where files from all sub-directories will be moved. If neither this nor the Directory parameter are set then the current location will be used.

#### `-WhatIf`
*Optional*. If supplied this will output a formatted table of the from and to file locations that will result from running the cmdlet.

#### `-DeleteSubDirectories` (alias -DS)
*Optional*. If supplied all sub-directories will be deleted once all files have been moved.

#### `-RenameMethod` (alias -RM)
*Optional*. The strategy for renaming duplicate files. Valid values are "Guid" or "Index". Defaults to Index.


