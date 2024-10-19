using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FlattenFolders.Models;

internal enum RenameStrategy
{
    Guid,
    Index
}

internal class FileProcessContainer
{
    internal FileProcessContainer()
    {
        Files = new List<SourceFile>();
        SourceDirectories = new List<string>();
        RenameOption = RenameStrategy.Guid; // Default strategy
    }

    internal List<SourceFile> Files { get; set; }
    internal List<string> SourceDirectories { get; set; }
    internal List<FileMapping> FileMappings { get; private set; }
    internal int SubDirectoryCount { get; set; }
    internal RenameStrategy RenameOption { get; set; } // New property

    internal void BuildDuplicatesAndFileMappings()
    {
        var duplicates = Files
            .GroupBy(f => f.Name)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        var duplicateIndexTracker = new Dictionary<string, int>();

        FileMappings = Files.Select(file =>
        {
            string fileName;

            if (duplicates.Contains(file.Name))
            {
                switch (RenameOption)
                {
                    case RenameStrategy.Guid:
                        fileName = $"{Path.GetFileNameWithoutExtension(file.Name)}_{Guid.NewGuid()}{Path.GetExtension(file.Name)}";
                        break;

                    case RenameStrategy.Index:
                        if (!duplicateIndexTracker.ContainsKey(file.Name))
                        {
                            duplicateIndexTracker[file.Name] = 1; // Start index at 1
                        }
                        else
                        {
                            duplicateIndexTracker[file.Name]++;
                        }

                        fileName = $"{Path.GetFileNameWithoutExtension(file.Name)}_{duplicateIndexTracker[file.Name]}{Path.GetExtension(file.Name)}";
                        break;

                    default:
                        fileName = file.Name;
                        break;
                }
            }
            else
            {
                fileName = file.Name;
            }

            return new FileMapping(file.File, Path.Combine(file.ParentDir, fileName));
        }).ToList();
    }
}