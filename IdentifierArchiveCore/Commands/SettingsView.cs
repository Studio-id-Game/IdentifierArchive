using StudioIdGames.IdentifierArchiveCore.Files;
using StudioIdGames.IdentifierArchiveCore.FolderControllers;
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

            Console.WriteLine($"Settings : \n{SettingsFile.Controller.ToBytes(rawView)}\n");
            Console.WriteLine($"Settings (replaced) : \n{SettingsFile.Controller.ToBytes(safeView)}\n");

            var localKeyFolderController = new LocalKeyFolderController(settings);

            var localKeyFileNames = localKeyFolderController.GetAllFilesName();

            if (localKeyFileNames == null)
            {
                return -1;
            }

            Console.WriteLine($"LocalkeyFiles : \n\t{string.Join("\n\t", localKeyFileNames)}\n");

            return 0;
        }
    }

}
