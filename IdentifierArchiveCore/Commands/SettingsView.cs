using StudioIdGames.IdentifierArchiveCore.Files;
using StudioIdGames.IdentifierArchiveCore.FolderControllers;
using System.Text;
using Utf8Json;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class SettingsView : CommandAction
    {
        public static SettingsView Instance { get; } = new SettingsView();

        private SettingsView() { }

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


            var settingsFolderController = new SettingsFolderController(args.SettingsFolder);
            var targetFolderController = new TargetFolderController(settingsFolderController, args.TargetFolder);
            var settings = settingsFolderController.GetSettingsFile(out var rawView, out var safeView, targetFolderController.FolderInfo, args.Identifier);
            
            if (settings == null || safeView == null || rawView == null)
            {
                return -1;
            }

            Console.WriteLine($"Settings : {rawView.AsText()}\n");
            Console.WriteLine($"Settings (replaced) : {safeView.AsText()}\n");

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
