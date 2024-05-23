using StudioIdGames.IdentifierArchiveCore.Commands;
using StudioIdGames.IdentifierArchiveCore.Files;

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
    }
}