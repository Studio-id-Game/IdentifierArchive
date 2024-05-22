using StudioIdGames.IdentifierArchiveCore.Files;
using StudioIdGames.IdentifierArchiveCore.FolderControllers;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public readonly struct SettingsInit : ICommandAction
    {
        public const string CommandID = "si";

        public const string Name = "Settings-Init";

        string ICommandAction.CommandID => CommandID;

        string ICommandAction.Name => Name;

        public int Excute(CommandArgs args)
        {
            if (!args.CheckRequire(this, settingsFodler: true))
            {
                return -1;
            }

            var folderController = new SettingsFolderController(args.SettingsFolder);

            if(!folderController.FolderSetup(args))
            {
                return -1;
            }

            var settings = folderController.GetSettingsFile();

            if(settings == null)
            {
                return -1;
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
