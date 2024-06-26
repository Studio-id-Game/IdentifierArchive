﻿using StudioIdGames.IdentifierArchiveCore.FolderControllers;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandActionTargetInit : CommandAction
    {
        public static CommandActionTargetInit Instance { get; } = new CommandActionTargetInit();

        private CommandActionTargetInit() { }

        public override string CommandID => "ti";

        public override string Name => "Target-Init";

        public override int Excute(CommandArgs args)
        {
            base.Excute(args);

            if (!args.CheckRequire(this, settingsFodler: true, targetFolder: true))
            {
                return -1;
            }

            var settingsFolderController = new SettingsFolderController(args.SettingsFolder!);
            var targetFolderController = new TargetFolderController(settingsFolderController, args.TargetFolder!);
            var settings = settingsFolderController.GetSettingsFile()
                ?.GetReplaced(targetFolderInfo: targetFolderController.FolderInfo, identifier: args.Identifier);

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
