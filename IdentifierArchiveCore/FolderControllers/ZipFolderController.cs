using StudioIdGames.IdentifierArchiveCore.Commands;
using StudioIdGames.IdentifierArchiveCore.Files;

namespace StudioIdGames.IdentifierArchiveCore.FolderControllers
{
    public class ZipFolderController(SettingsFolderController settingsFolderController, SettingsFile settings) : FolderController
    {
        public override string ScreenName => "ZipArchive";

        private static GitignoreFile Gitignore => new(GitignoreFileType.Zip);

        public override DirectoryInfo FolderInfo => settings.GetZipFileInfo(settingsFolderController.FolderInfo).Directory!;

        public override bool FolderSetup(CommandArgs args)
        {
            return base.FolderSetup(args) && GitignoreSetup();
        }

        private bool GitignoreSetup()
        {
            return Gitignore.ToFile(FolderInfo, out var created, out _, true, false) || created;
        }
    }
}