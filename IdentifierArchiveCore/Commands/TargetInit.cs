using StudioIdGames.IdentifierArchiveCore.FolderControllers;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class TargetInit : ICommandAction
    {
        public const string CommandID = "ti";

        public const string Name = "Target-Init";

        string ICommandAction.CommandID => CommandID;

        string ICommandAction.Name => Name;

        public int Excute(CommandArgs args)
        {
            if (!args.CheckRequire(this, settingsFodler: true, targetFolder: true))
            {
                return -1;
            }

            var settingsFolderController = new SettingsFolderController(args.SettingsFolder);
            var targetFolderController = new TargetFolderController(settingsFolderController, args.TargetFolder);
            var settings = settingsFolderController.GetSettingsFile(targetFolderController.FolderInfo, args.Identifier);

            if (settings == null)
            {
                return -1;
            }

            if (!targetFolderController.FolderSetup(args))
            {
                return -1;
            }

            return 0;
        }
    }

}
