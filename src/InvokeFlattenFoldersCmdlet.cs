using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using FMFlattenFolders.Models;

namespace FMFlattenFolders {
    /// <summary>
    /// <para type="synopsis">
    /// Moves all containing files including those within all sub-directories of a supplied directory to the root directory and optionally deletes empty sub-directories after the operation.
    /// </para>
    /// <para type="description">
    /// Moves all containing files including those within all sub-directories of a supplied directory to the root directory and optionally deletes empty sub-directories after the operation.
    /// Supports renaming duplicate files with a unique GUID, or a numerical index. The index can be zero-padded to a custom length. Supports multiple directories with the -Directories parameter.
    /// Also supports pipeline input: Multiple directories can be piped into the command.
    /// </para>
    /// <example>
    ///     <para>All files in all sub-directories of "C:\Icons\SVG Sets" will be moved to "C:\Icons\SVG Sets". Duplicate files will be renamed with an appended index. All remaining empty subdirectories will be deleted once the files have been moved.:</para>
    ///     <code>PS C:\> Invoke-FlattenFolders -Directory "C:\Icons\SVG Sets" -RenameMethod Index -DeleteSubdirectories</code>
    /// </example>
    /// <example>
    ///     <para>Displays an output table to the console detailing the flattening operation without actually modifying any files on disk.:</para>
    ///     <code>PS C:\> Invoke-FlattenFolders -Directory "C:\Videos" -WhatIf</code>
    /// </example>
    /// <example>
    ///     <para>All files in all sub-directories in "C:\Videos" will be moved to "C:\Videos", and all files in all sub-directories of "C:\Music" will be moved to "C:\Music". Duplicate files will be renamed with an appended GUID. All remaining empty subdirectories will be deleted once the files have been moved.:</para>
    ///     <code>PS C:\> Invoke-FlattenFolders -Directories "C:\Videos", "C:\Music" -RenameMethod GUID -DeleteSubdirectories</code>
    /// </example>
    /// <example>
    ///     <para>All files in all sub-directories in the piped array of directories( "C:\Videos\" and "C:\Music\" ) will be moved to their respective parent folders. All remaining empty subdirectories will be deleted once the files have been moved.:</para>
    ///     <code>PS C:\> "C:\Videos\","C:\Music\" | Invoke-FlattenFolders -DeleteSubdirectories</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsLifecycle.Invoke, "FlattenFolders", HelpUri = "https://github.com/futuremotiondev/FMFlattenFolders")]
    public class InvokeFlattenFoldersCmdlet : PSCmdlet {
        private const string SingleDirectory = "SingleDirectory";
        private const string MultipleDirectories = "MultipleDirectories";

        #region Parameters
        /// <summary>
        /// <para type="description">
        /// The parent directory where files from all sub-directories will be moved.
        /// If neither this nor the Directories parameter are set then the current location will be used.
        /// </para>
        /// /// </summary>
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = SingleDirectory)]
        [Alias("dir")]
        public string Directory { get; set; }

        /// <summary>
        /// <para type="description">
        /// A collection of parent directories where files from all sub-directories will be moved.
        /// If neither this nor the Directory parameter are set then the current location will be used.
        /// </para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = MultipleDirectories)]
        [Alias("dirs")]
        public List<string> Directories { get; set; }

        /// <summary>
        /// <para type="description">
        /// The rename strategy to use for duplicate files. Valid values are GUID or Index.
        /// GUID will append a GUID to duplicate files.
        /// Index will append a numeric index to duplicate files.
        /// </para>
        /// </summary>
        [Parameter(Mandatory = false)]
        [Alias("method", "m")]
        [ValidateSet("GUID", "Index")]
        public string RenameMethod { get; set; }

        /// <summary>
        /// <para type="description">
        /// The amount of zero padding to apply to indexes when the RenameMethod is set to 'Index'
        /// </para>
        /// </summary>
        [Parameter(Mandatory = false)]
        [Alias("pad", "p")]
        [ValidateRange(1, 6)]
        public int IndexZeroPadding { get; set; } = 2;

        /// <summary>
        /// <para type="description">
        /// If supplied all subdirectories will be deleted once all files have been moved.
        /// </para>
        /// </summary>
        [Parameter(Mandatory = false)]
        [Alias("del", "d")]
        public SwitchParameter DeleteSubdirectories { get; set; }

        /// <summary>
        /// <para type="description">
        /// If supplied this will output a formatted table of the from and to file locations that will result from running the cmdlet.
        /// </para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter WhatIf { get; set; }

        #endregion Parameters

        private bool _isValid;
        private FileProcessContainer _container;

        // Internal method to process the cmdlet logic.
        internal void ProcessInternal() {
            BeginProcessing();
            ProcessRecord();
            EndProcessing();
        }

        /// <summary>
        /// Implements the <see cref="BeginProcessing"/> method for <see cref="InvokeFlattenFoldersCmdlet"/>.
        /// Initializes temporary containers at the beginning of processing.
        /// </summary>
        protected override void BeginProcessing() {
            _container = new FileProcessContainer();
            _isValid = true;
        }

        /// <summary>
        /// Implements the <see cref="ProcessRecord"/> method for <see cref="InvokeFlattenFoldersCmdlet"/>.
        /// Validates input directories exist and prepares them for processing. Builds a list of directories to process in the EndProcessing method.
        /// </summary>
        protected override void ProcessRecord() {
            // Determine directories to check based on parameter set.
            var directoriesToCheck = ParameterSetName == MultipleDirectories ? Directories : [Directory];

            // Validate each directory exists.
            foreach (var d in directoriesToCheck) {
                if (!System.IO.Directory.Exists(d)) {
                    _isValid = false;
                    ThrowTerminatingError(new ErrorRecord(
                        new DirectoryNotFoundException($"Directory not found: {d}"),
                        "DirectoryNotFound",
                        ErrorCategory.InvalidArgument,
                        d));
                }
            }
            // If all directories are valid, add them to the container.
            if (_isValid) {
                _container.SourceDirectories.AddRange(directoriesToCheck);
            }
        }

        /// <summary>
        /// Implements the <see cref="EndProcessing"/> method for <see cref="InvokeFlattenFoldersCmdlet"/>.
        /// Performs the folder flattening operation on validated directories.
        /// </summary>
        protected override void EndProcessing() {
            if (!_isValid) return;

            // Collect files and count subdirectories for each source directory.
            foreach (var d in _container.SourceDirectories) {
                var files = System.IO.Directory.GetFiles(d, "*", SearchOption.AllDirectories);
                _container.Files.AddRange(files.Select(f => new SourceFile(d, f, Path.GetFileName(f))));
                _container.SubDirectoryCount += System.IO.Directory.GetDirectories(d, "*", SearchOption.AllDirectories).Length;
            }

            // Terminate if no files are found.
            if (_container.Files.Count == 0) {
                ThrowTerminatingError(new ErrorRecord(
                    new Exception("No files found, terminating."),
                    "NoFilesFound",
                    ErrorCategory.ResourceUnavailable,
                    null));
            }

            // Set rename strategy and build file mappings for duplicates.
            _container.RenameOption = RenameMethod == "Index" ? RenameStrategy.Index : RenameStrategy.Guid;
            _container.BuildDuplicatesAndFileMappings();

            // If 'WhatIf' is specified, simulate the operation.
            if (WhatIf.IsPresent) {
                ProcessWhatIf();
                return;
            }

            // Move all files to their respective parent directories.
            foreach (FileMapping fileMapping in _container.FileMappings) {
                File.Move(fileMapping.OldFile, fileMapping.NewFile);
            }

            // Optionally delete subdirectories if specified.
            if (DeleteSubdirectories.IsPresent) {
                foreach (var d in _container.SourceDirectories) {
                    foreach (var sub in System.IO.Directory.GetDirectories(d)) {
                        System.IO.Directory.Delete(sub, true);
                    }
                }
            }
        }

        /// <summary>
        /// Simulates the operation and outputs expected results when 'WhatIf' is used.
        /// </summary>
        private void ProcessWhatIf() {
            // Prepare descriptive text for output.
            string strFile        = (_container.Files.Count == 1) ? "file" : "files";
            string strSubdir      = (_container.SubDirectoryCount == 1) ? "subdirectory" : "subdirectories";
            string strDir         = (_container.SourceDirectories.Count == 1) ? "parent directory" : "parent directories";
            string strDelSubdirs  = (DeleteSubdirectories) ? "and all sub-directories would be deleted." : "";
            string filesText      = $"{_container.Files.Count} {strFile}";
            string subDirText     = $"{_container.SubDirectoryCount} {strSubdir}";
            string dirText        = $"{_container.SourceDirectories.Count} {strDir} {strDelSubdirs}";

            // Map parent directories for display.
            List<(int ix, string dir)> parentDirMappings = _container.SourceDirectories
                .Select((d, i) => (ix: i, dir: d))
                .OrderBy(o => o.ix)
                .ToList();

            // Function to replace file paths with parent directory placeholders.
            string ReplaceParentDir(string file) =>
                parentDirMappings.Aggregate(file, (current, mapping) => current.Replace(mapping.dir.TrimEnd('/', '\\'), $"[Parent {mapping.ix}]"));

            // Build the output table using StringBuilder for efficiency.
            var output = new StringBuilder();
            output.AppendLine()
                .AppendLine($"{filesText} would be moved from {subDirText} into {dirText}")
                .AppendLine()
                .AppendLine("The following table shows file moves where:")
                .AppendLine();

            // Append parent directory mappings to the output.
            foreach (var m in parentDirMappings) {
                output.AppendLine($"[Parent {m.ix}] = {m.dir}");
            }

            // Calculate maximum lengths for formatting.
            int maxOldStringLength = _container.FileMappings.Max(f => ReplaceParentDir(f.OldFile).Length);
            int maxNewStringLength = _container.FileMappings.Max(f => ReplaceParentDir(f.NewFile).Length);

            // Append formatted table headers and rows.
            output.AppendLine()
                .AppendLine($"|={"".PadRight(maxOldStringLength, '=')}==={"".PadRight(maxNewStringLength, '=')}=|")
                .AppendLine($"| {"Old file".PadRight(maxOldStringLength)} | {"New file".PadRight(maxNewStringLength)} |")
                .AppendLine($"|-{"".PadRight(maxOldStringLength, '-')}---{"".PadRight(maxNewStringLength, '-')}-|");

            foreach (var f in _container.FileMappings) {
                output.AppendLine($"| {ReplaceParentDir(f.OldFile).PadRight(maxOldStringLength, ' ')} | {ReplaceParentDir(f.NewFile).PadRight(maxNewStringLength, ' ')} |");
            }
            output.AppendLine($"|={"".PadRight(maxOldStringLength, '=')}==={"".PadRight(maxNewStringLength, '=')}=|").AppendLine();

            // Output the constructed table.
            WriteObject(output.ToString(), true);
        }
    }
}