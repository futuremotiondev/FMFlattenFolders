<img src="https://raw.githubusercontent.com/futuremotiondev/FMFlattenFolders/refs/heads/main/assets/FMFlattenFoldersLogo.png" alt="Description" width="180">

# FMFlattenFolders

This is a modified version of FlattenFolders by [Rob Green](https://github.com/trossr32/ps-flatten-folders). I've added an additional method of renaming duplicate files by inserting a numerical index as a suffix, as well as control of the zero-padding for said suffix.

The original module only renames duplicates by adding a GUID. In my opinion this is kind of heavy-handed. But the option is still available if desired.

<a href="https://www.powershellgallery.com/packages/FMFlattenFolders"><img src="https://raw.githubusercontent.com/futuremotiondev/FMFlattenFolders/refs/heads/main/assets/gallery-badge.png" alt="Description" height="26"></a>
<a href="./LICENSE.md"><img src="https://raw.githubusercontent.com/futuremotiondev/FMFlattenFolders/refs/heads/main/assets/license-badge.png" alt="Description" height="26"></a>

## Installation (from the Powershell Gallery)

```powershell
Install-Module FMFlattenFolders
Import-Module FMFlattenFolders
```

## Description

Moves files from all sub-directories to the parent directory. If files with duplicate names are found, they will have either a GUID or a numeric index appended to the file name depending on the passed value of `-RenameMethod`.

The cmdlet also Supports `-WhatIf`. If supplied this will output a formatted table of the final locations of all files.

## Changelog

### v2.0.5

1. Changed `-RenameMethod Index` behavior so that the first file in a set of duplicate files does not get a suffix.

### v2.0.4

1. Fixed incorrect handling of duplicate indexes.
2. Fixed case where empty subdirectories were not being deleted despite `-DeleteSubdirectories` being specified.
3. Optimized and reduced unneeded redundancy in much of the code
4. Added some more descriptive comments in code.

## Usage

### Example 1

```powershell
Invoke-FlattenFolders -Directory "C:\Icons\SVG Sets" -RenameMethod Index -DeleteSubdirectories
```

All files in all sub-directories of "C:\Icons\SVG Sets" will be moved to "C:\Icons\SVG Sets". Duplicate files will be renamed with an appended index. All remaining empty subdirectories will be deleted once the files have been moved.

### Example 2

```powershell
Invoke-FlattenFolders -Directories "C:\Videos", "C:\Music" -RenameMethod GUID -DeleteSubdirectories
```

All files in all sub-directories in "C:\Videos" will be moved to "C:\Videos", and all files in all sub-directories of "C:\Music" will be moved to "C:\Music". Duplicate files will be renamed with an appended GUID. All remaining empty subdirectories will be deleted once the files have been moved.

### Example 3

```powershell
"C:\Videos\","C:\Music\" | Invoke-FlattenFolders -DeleteSubdirectories
```

All files in all sub-directories in the piped array of directories( "C:\Videos\" and "C:\Music\" ) will be moved to their respective parent folders. All remaining empty subdirectories will be deleted once the files have been moved.

### Example 4

```powershell
Invoke-FlattenFolders -Directory "C:\Videos" -WhatIf
```

Displays an output table to the console detailing the flattening operation without actually modifying any files on disk.

## Parameters

`-Directory` (alias `-dir`)

**Optional**. The parent directory where files from all sub-directories will be moved. If neither this nor the Directories parameter are set then the current location will be used.

`-Directories` (alias `-dirs`)

**Optional**. A collection of parent directories where files from all sub-directories will be moved. If neither this nor the Directory parameter are set then the current location will be used.

`-RenameMethod` (alias `-method`, `-m`)

**Optional**. The strategy for renaming duplicate files. Valid values are "Guid" or "Index". Defaults to Index.

`-IndexZeroPadding` (alias `-pad`, `-p`)

**Optional**. The amount of zero padding to apply to indexes when the RenameMethod is set to 'Index'. For instance, if `-IndexZeroPadding` is set to 3, duplicates will have suffixes like `file_002`, `file_003`, etc.

`-DeleteSubDirectories` (alias `-del`, `-d`)

**Optional**. If supplied all sub-directories will be deleted once all files have been moved.

`-WhatIf`

**Optional**. If supplied this will output a formatted table of the from and to file locations that will result from running the cmdlet.






