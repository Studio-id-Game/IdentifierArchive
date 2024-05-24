using StudioIdGames.IdentifierArchiveCore.Commands;
using System;
using System.Diagnostics;

namespace StudioIdGames.IdentifierArchiveCore.FolderControllers
{
    public abstract class FolderController
    {
        public static IEnumerable<string>? GetRelativePath<T>(string relativeTo, params T[] infos) where T : FileSystemInfo
        {
            return infos.Select(e => Path.GetRelativePath(relativeTo, e.FullName));
        }

        public static IEnumerable<string>? GetRelativePath<T>(string relativeTo,IEnumerable<T> infos) where T : FileSystemInfo
        {
            return infos.Select(e => Path.GetRelativePath(relativeTo, e.FullName));
        }

        public static int GitFileList(string gitExePath, DirectoryInfo dir, out IEnumerable<FileInfo> gitFiles, out IEnumerable<FileInfo> ignoredFiles, bool showInfo = true)
        {
            var args = "ls-files --cached --others --exclude-standard";
            Console.WriteLine($"Check git target files... ({gitExePath} {args})");
            var psInfo = new ProcessStartInfo(gitExePath, args)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                WorkingDirectory = dir.FullName,
            };

            var files = dir.EnumerateFiles("*", SearchOption.AllDirectories)
                .Where(file => file.Name != ".git")
                .ToList();

            using var process = Process.Start(psInfo);

            if (process == null)
            {
                gitFiles = [];
                ignoredFiles = [];
                Console.WriteLine($"Failed to check git target files.\n");
                return -1;
            }

            var targetsList = new List<FileInfo>();

            while (!process.StandardOutput.EndOfStream)
            {
                var line = process.StandardOutput.ReadLine();
                if (line != null)
                {
                    var index = files.FindIndex(file => file.Name == line);
                    if (index >= 0)
                    {
                        targetsList.Add(files[index]);
                        files.RemoveAt(index);
                    }
                }
            }

            if (!process.WaitForExit(100000))
            {
                process.Kill();
                gitFiles = [];
                ignoredFiles = [];
                Console.WriteLine($"Failed to check git target files.\n");
                return -1;
            }

            gitFiles = targetsList;
            ignoredFiles = files;

            var exitCode = process.ExitCode;

            Console.WriteLine($"Checked git target files. (exit code: {exitCode})\n");

            if (showInfo)
            {
                foreach (var item in gitFiles)
                {
                    Console.WriteLine($"\tGitFile: {item.FullName}");
                }

                foreach (var item in ignoredFiles)
                {
                    Console.WriteLine($"\tIgnored: {item.FullName}");
                }

                Console.WriteLine();
            }

            return exitCode;
        }

        public abstract DirectoryInfo FolderInfo { get; }

        public abstract string ScreenName { get; }

        public virtual bool FolderSetup(CommandArgs args)
        {
            Console.WriteLine($"{ScreenName} folder setup start. ({FolderInfo.FullName})\n");
            return ConsoleUtility.CheckFolder(FolderInfo, ScreenName, out var created, args.AutoFolderCreate) || created;
        }

        protected void FolderSetupEnd(bool result)
        {
            if (result)
            {
                Console.WriteLine($"{ScreenName} folder setup succeeded. ({FolderInfo.FullName})\n");
            }
            else
            {
                Console.WriteLine($"{ScreenName} folder setup failed. ({FolderInfo.FullName})\n");
            }
        }

        public bool CheckFolder()
        {
            return ConsoleUtility.CheckFolder(FolderInfo, ScreenName);
        }

        public IEnumerable<FileInfo>? GetAllFiles()
        {
            if (!CheckFolder())
            {
                return null;
            }

            return FolderInfo.EnumerateFiles("*", SearchOption.AllDirectories);
        }

        public IEnumerable<string>? GetRelativePath<T>(params T[] infos) where T : FileSystemInfo
        {
            return GetRelativePath(FolderInfo.FullName, infos);
        }

        public IEnumerable<string>? GetRelativePath<T>(IEnumerable<T> infos) where T : FileSystemInfo
        {
            return GetRelativePath(FolderInfo.FullName, infos);
        }

        public IEnumerable<string>? GetAllFilesName()
        {
            var files = GetAllFiles();
            if (files == null)
            {
                return null;
            }

            return GetRelativePath(files);
        }

        public int GitFileList(string gitExePath, out IEnumerable<FileInfo> gitFiles, out IEnumerable<FileInfo> ignoredFiles, bool showInfo = true)
        {
            return GitFileList(gitExePath, FolderInfo, out gitFiles, out ignoredFiles, showInfo);
        }

        public int GitFileNameList(string gitExePath, out IEnumerable<string>? gitFileNames, out IEnumerable<string>? ignoredFileNames, bool showInfo = true)
        {
            var ret = GitFileList(gitExePath, out var gitFiles, out var ignoredFiles, showInfo);
            
            if(ret < 0)
            {
                gitFileNames = [];
                ignoredFileNames = [];
                return ret;
            }

            gitFileNames = GetRelativePath(gitFiles);
            ignoredFileNames = GetRelativePath(ignoredFiles);

            return ret;
        }

        public override string ToString()
        {
            return $"{ScreenName} ({FolderInfo.FullName})";
        }
    }
}