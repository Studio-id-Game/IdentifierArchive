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
            return ConsoleUtility.CheckFolder(FolderInfo, ScreenName, out var created, args.AutoFolderCreate) || created;
        }

        public bool CheckFolder()
        {
            return ConsoleUtility.CheckFolder(FolderInfo, ScreenName);
        }
    }
}