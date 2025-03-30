using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FMFlattenFolders.Models {
    internal enum RenameStrategy {
        Guid = 0,
        Index = 1
    }

    internal class FileProcessContainer {
        internal FileProcessContainer() {
            Files = [];
            SourceDirectories = [];
            FileMappings = [];
            IndexZeroPadding = 2;
            RenameOption = RenameStrategy.Index; // Default rename strategy
        }

        internal List<SourceFile> Files { get; set; }
        internal List<string> SourceDirectories { get; set; }
        internal List<FileMapping> FileMappings { get; private set; }
        internal int SubDirectoryCount { get; set; }
        internal int IndexZeroPadding { get; set; }
        internal RenameStrategy RenameOption { get; set; }

        internal void BuildDuplicatesAndFileMappings() {
            // Identify duplicate file names
            List<string> duplicates = Files
                .GroupBy(f => f.Name)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            // Initialize dictionary correctly
            Dictionary<string, int> duplicateIndexTracker = [];

            // Use LINQ Select for clarity and efficiency

            FileMappings = Files.Select(file => {
                string fileName = duplicates.Contains(file.Name) ?
                    RenameOption switch {
                        RenameStrategy.Guid => $"{Path.GetFileNameWithoutExtension(file.Name)}_{Guid.NewGuid()}{Path.GetExtension(file.Name)}",
                        RenameStrategy.Index => GetUniqueNameIfDuplicate(file.Name, duplicateIndexTracker, IndexZeroPadding),
                        _ => file.Name
                    } : file.Name;

                return new FileMapping(file.File, Path.Combine(file.ParentDir ?? string.Empty, fileName));
            }).ToList(); // Convert to list after selection
        }

        internal static string GetUniqueNameIfDuplicate(string fileName, Dictionary<string, int> duplicateIndexTracker, int padding) {
            if (!duplicateIndexTracker.TryGetValue(fileName, out int currentIndex)) {
                // Key does not exist, initialize it
                duplicateIndexTracker[fileName] = 1; // Start index at 1
            }
            else {
                // Key exists, increment the index
                duplicateIndexTracker[fileName] = currentIndex + 1;
            }
            // Format the index with zero padding
            string paddedIndex = duplicateIndexTracker[fileName].ToString($"D{padding}");
            return $"{Path.GetFileNameWithoutExtension(fileName)}_{paddedIndex}{Path.GetExtension(fileName)}";
        }
    }
}