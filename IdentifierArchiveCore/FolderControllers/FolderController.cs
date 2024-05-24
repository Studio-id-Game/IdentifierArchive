using StudioIdGames.IdentifierArchiveCore.Commands;
using System;
using System.Diagnostics;

namespace StudioIdGames.IdentifierArchiveCore.FolderControllers
{
    public abstract class FolderController
    {
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

        public FileInfo[]? GetAllFiles()
        {
            if (!CheckFolder())
            {
                return null;
            }

            return FolderInfo.GetFiles("*", SearchOption.AllDirectories);
        }

        public string[]? GetAllFilesName(string relativeTo)
        {
            var files = GetAllFiles();
            if (files == null)
            {
                return null;
            }

            return files.Select(e => Path.GetRelativePath(relativeTo, e.FullName)).ToArray();
        }

        public string[]? GetAllFilesName()
        {
            return GetAllFilesName(FolderInfo.FullName);
        }

        public override string ToString()
        {
            return $"{ScreenName} ({FolderInfo.FullName})";
        }

        public int GitFileList(string gitExePath, out IEnumerable<FileInfo> gitFiles, out IEnumerable<FileInfo> ignoredFiles)
        {
            return GitFileList(gitExePath, FolderInfo, out gitFiles, out ignoredFiles);    
        }

        public static int GitFileList(string gitExePath, DirectoryInfo dir, out IEnumerable<FileInfo> gitFiles, out IEnumerable<FileInfo> ignoredFiles, bool showInfo = true)
        {
            var psInfo = new ProcessStartInfo(gitExePath, "ls-files --cached --others --exclude-standard")
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

            if(process == null)
            {
                gitFiles = [];
                ignoredFiles = [];
                return -1;
            }

            var targetsList = new List<FileInfo>();

            while (!process.StandardOutput.EndOfStream)
            {
                var line = process.StandardOutput.ReadLine();
                if (line != null)
                {
                    var index = files.FindIndex(file => file.Name == line);
                    if(index >= 0)
                    {
                        targetsList.Add(files[index]);
                        files.RemoveAt(index);
                    }
                }
            }

            if (!process.WaitForExit(100000))
            {
                process.Kill();
            }

            gitFiles = targetsList;
            ignoredFiles = files;

            var exitCode = process.ExitCode;

            if(showInfo)
            {
                Console.WriteLine($"Check git target files. (exit code: {exitCode})\n");

                foreach (var item in gitFiles)
                {
                    var relativePath = Path.GetRelativePath(dir.FullName, item.FullName);
                    Console.WriteLine($"\tGitFile: {relativePath}");
                }

                foreach (var item in ignoredFiles)
                {
                    var relativePath = Path.GetRelativePath(dir.FullName, item.FullName);
                    Console.WriteLine($"\tIgnored: {relativePath}");
                }

                Console.WriteLine();
            }

            return exitCode;
        }
    }
}