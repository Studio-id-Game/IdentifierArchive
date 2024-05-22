using StudioIdGames.IdentifierArchiveCore.FolderControllers;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class TargetView : CommandAction
    {
        public static TargetView Instance { get; } = new TargetView();

        private TargetView() { }

        public override string CommandID => "tv";

        public override string Name => "Target-View";

        public override int Excute(CommandArgs args)
        {
            base.Excute(args);

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

            var archiveIdentifier = targetFolderController.GetArchiveIdentifier(true)?.Text ?? "(null)";
            var currentIdentifier = targetFolderController.GetCurrentIdentifier(true)?.Text ?? "(null)";

            Console.WriteLine($"Archive Identifier: {archiveIdentifier}");
            Console.WriteLine($"Current Identifier: {currentIdentifier}");
            
            var ignores = new string[] { ".gitignore", "~", ".identifier" };
            var filenames = (targetFolderController.GetAllFilesName() ?? []).Where(s => ignores.All(i => !s.EndsWith(i)));

            Console.WriteLine($"Files: \n\t{string.Join("\n\t", filenames)}");

            return 0;
        }
    }
}
