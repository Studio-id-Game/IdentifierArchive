using StudioIdGames.IdentifierArchiveCore.FolderControllers;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandActionRemoteDownload : CommandAction
    {
        public static CommandActionRemoteDownload Instance { get; } = new CommandActionRemoteDownload();

        private CommandActionRemoteDownload() { }

        public override string CommandID => "dl";

        public override string Name => "Download-From-Remote";

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
                Console.WriteLine("Backup archive files cannot be downloaded.\n");
            }

            var settingsWithOutIdentifier = settingsFolderController.GetSettingsFile()
                ?.GetReplaced(targetFolderInfo: targetFolderController.FolderInfo);

            if (settingsWithOutIdentifier == null)
            {
                return -1;
            }

            var zipFolderController = new ZipFolderController(settingsWithOutIdentifier);

            return zipFolderController.DownloadZipFile(identifier, args.AutoFileOverwrite);
        }
    }
}
