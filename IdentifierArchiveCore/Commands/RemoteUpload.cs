using StudioIdGames.IdentifierArchiveCore.Files;
using StudioIdGames.IdentifierArchiveCore.FolderControllers;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class RemoteUpload : CommandAction
    {
        public static RemoteUpload Instance { get; } = new RemoteUpload();

        private RemoteUpload() { }

        public override string CommandID => "ul";

        public override string Name => "Upload-To-Remote";

        public override int Excute(CommandArgs args)
        {
            base.Excute(args);

            if (!args.CheckRequire(this, settingsFodler: true, targetFolder: true))
            {
                return -1;
            }

            var settingsFolderController = new SettingsFolderController(args.SettingsFolder!);

            var targetFolderController = new TargetFolderController(settingsFolderController, args.TargetFolder!);
            var identifier = string.IsNullOrWhiteSpace(args.Identifier) ? targetFolderController.GetArchiveIdentifier(true)?.Text : args.Identifier;

            if (identifier == null)
            {
                Console.WriteLine("Not found ArchiveIdentifier file of not set -id augument.\n");
                return -1;
            }

            if (identifier == ZipFolderController.BackupIdentifier)
            {
                Console.WriteLine("Backup archive files cannot be uploaded.\n");
            }

            var settingsWithOutIdentifier = settingsFolderController.GetSettingsFile()
                ?.GetReplaced(targetFolderInfo: targetFolderController.FolderInfo);

            if (settingsWithOutIdentifier == null)
            {
                return -1;
            }

            var zipFolderController = new ZipFolderController(settingsWithOutIdentifier);

            return zipFolderController.UoloadZipFile(identifier);
        }
    }
}
