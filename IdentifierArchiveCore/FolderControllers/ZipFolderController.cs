using StudioIdGames.IdentifierArchiveCore.Commands;
using StudioIdGames.IdentifierArchiveCore.Files;

namespace StudioIdGames.IdentifierArchiveCore.FolderControllers
{
    public class ZipFolderController : FolderController
    {
        public const string BackupIdentifier = "IdentifierArchiveSystem_AutoBackup";
        private readonly SettingsFile settings;

        public ZipFolderController(SettingsFile settingsWithoutIdentifier, string identifier)
        {
            var settings = new SettingsFile();
            settings.CopyFrom(settingsWithoutIdentifier);
            settings.Replace(out _, identifier: identifier);

            this.settings = settings;
        }

        public ZipFolderController(SettingsFile settings)
        {
            this.settings = settings;
        }

        public override string ScreenName => "ZipArchive";

        private static GitignoreFile Gitignore => new(GitignoreFileType.Zip);

        public override DirectoryInfo FolderInfo => settings.GetZipFileInfo().Directory!;

        public override bool FolderSetup(CommandArgs args)
        {
            var ret = base.FolderSetup(args) && GitignoreSetup();
            FolderSetupEnd(ret);
            return ret;
        }

        public FileInfo GetZipFileInfo() => settings.GetZipFileInfo();

        public bool DeleteCache(IEnumerable<string> unsafeFileNames, bool? autoFileOverwrite)
        {
            var info = settings.GetZipFileInfo();
            return ConsoleUtility.DeleteFile(info, unsafeFileNames, autoFileOverwrite);
        }

        private bool GitignoreSetup()
        {
            return Gitignore.ToFile(FolderInfo, out var created, out _, true, false) || created;
        }
    }
}