using StudioIdGames.IdentifierArchiveCore.FolderControllers;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandActionSettingsView : CommandAction
    {
        public static CommandActionSettingsView Instance { get; } = new CommandActionSettingsView();

        private CommandActionSettingsView() { }

        public override string CommandID => "sv";

        public override string Name => "Settings-View";

        public override int Excute(CommandArgs args)
        {
            base.Excute(args);

            if (!args.CheckRequire(this, settingsFodler: true))
            {
                return -1;
            }

            args = args.Copy();

            if (string.IsNullOrWhiteSpace(args.TargetFolder))
            {
                args.TargetFolder = "/SAMPLE_TARGET/SAMPLE_TARGET_SUBFOLDER/";
            }

            if (string.IsNullOrWhiteSpace(args.Identifier))
            {
                args.Identifier = "SAMPLE_IDENTIFIER";
            }


            var settingsFolderController = new SettingsFolderController(args.SettingsFolder!);
            var targetFolderController = new TargetFolderController(settingsFolderController, args.TargetFolder);
            var settingsRaw = settingsFolderController.GetSettingsFile();
            if (settingsRaw == null)
            {
                return -1;
            }

            var settings = settingsRaw.GetReplaced(targetFolderInfo: targetFolderController.FolderInfo, identifier: args.Identifier);

            Console.WriteLine($"Settings : {settingsRaw.SafeView?.AsText()}\n");
            Console.WriteLine($"Settings (replaced) : {settings.SafeView?.AsText()}\n");

            if (args.UnSafe && ConsoleUtility.Question("View unsafe data? (include local key data)", null))
            {
                Console.WriteLine($"Settings (unsafe replaced) : {settings.AsText()}\n");

                var localKeyFolderController = new LocalKeyFolderController(settings);

                var localKeyFileNames = localKeyFolderController.GetAllFilesName();

                if (localKeyFileNames == null)
                {
                    return -1;
                }

                Console.WriteLine($"LocalkeyFiles : \n\t{string.Join("\n\t", localKeyFileNames)}\n");
            }

            return 0;
        }
    }

}
