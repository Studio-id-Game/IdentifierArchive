using StudioIdGames.IdentifierArchiveCore.Commands;
using StudioIdGames.IdentifierArchiveCore.Files;

namespace StudioIdGames.IdentifierArchiveCore.FolderControllers
{
    public class ZipFolderController(SettingsFile settings) : FolderController
    {
        public override string ScreenName => "ZipArchive";

        private static GitignoreFile Gitignore => new(GitignoreFileType.Zip);

        public override DirectoryInfo FolderInfo => settings.GetZipFileInfo().Directory!;

        public override bool FolderSetup(CommandArgs args)
        {
            var ret = base.FolderSetup(args) && GitignoreSetup();
            FolderSetupEnd(ret);
            return ret;
        }

        private bool GitignoreSetup()
        {
            return Gitignore.ToFile(FolderInfo, out var created, out _, true, false) || created;
        }
    }
}