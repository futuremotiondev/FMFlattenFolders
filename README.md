# FMFlattenFolders

This is a modified version of [FlattenFolders](https://www.powershellgallery.com/packages/FlattenFolders) by Rob Green.

## Description

Moves files from all sub-directories to the parent directory. If files with duplicate names are found, they will have either a GUID or a numeric index appended to the file name.

Supports `-WhatIf`. If supplied this will output a formatted table of the from and to file locations that will result from running the cmdlet.

Can be run against:

* a single directory
* a collection of directories piped into the module.

## Installation (from the Powershell Gallery)

```powershell
Install-Module FMFlattenFolders
Import-Module FMFlattenFolders
```

## Parameters

#### -Directory (alias -D)
*Optional*. The parent directory where files from all sub-directories will be moved. If neither this nor the Directories parameter are set then the current location will be used.

#### -Directories
*Optional*. A collection of parent directories where files from all sub-directories will be moved. If neither this nor the Directory parameter are set then the current location will be used.

#### -WhatIf
*Optional*. If supplied this will output a formatted table of the from and to file locations that will result from running the cmdlet.

#### -DeleteSubDirectories (alias -DS)
*Optional*. If supplied all sub-directories will be deleted once all files have been moved.

#### -RenameMethod (alias -RM)
*Optional*. The strategy for renaming duplicate files. Valid values are "Guid" or "Index". Defaults to Index.

## Notes

This is a modified version of FlattenFolders by [Rob Green](https://github.com/trossr32/ps-flatten-folders).
I've simply added an additional method of renaming duplicate files.