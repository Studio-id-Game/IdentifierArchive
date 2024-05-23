using StudioIdGames.IdentifierArchiveCore.Files;
using StudioIdGames.IdentifierArchiveCore.FolderControllers;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class SettingsInit : CommandAction
    {
        public static SettingsInit Instance { get; } = new SettingsInit();

        private SettingsInit() { }

        public override string CommandID => "si";

        public override string Name => "Settings-Init";

        public override int Excute(CommandArgs args)
        {
            base.Excute(args);

            if (!args.CheckRequire(this, settingsFodler: true))
            {
                return -1;
            }

            var folderController = new SettingsFolderController(args.SettingsFolder);

            if(!folderController.FolderSetup(args))
            {
                return -1;
            }

            Console.WriteLine("Settings file check start.\n");

            var settings = folderController.GetSettingsFile(loadLocalkey: false);

            if (settings == null)
            {
                Console.WriteLine("Settings file check failed.\n");
                return -1;
            }
            else
            {
                Console.WriteLine("Settings file check succeeded.\n");
            }

            var localkeyFolderController = new LocalKeyFolderController(settings);

            if (!localkeyFolderController.FolderSetup(args))
            {
                return -1;
            }

            return 0;
        }
    }
}
