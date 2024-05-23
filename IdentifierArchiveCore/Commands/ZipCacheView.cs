using StudioIdGames.IdentifierArchiveCore.FolderControllers;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class ZipCacheView : CommandAction
    {
        public static ZipCacheView Instance { get; } = new ZipCacheView();

        private ZipCacheView() { }

        public override string CommandID => "cv";

        public override string Name => "Zip-Cache-View";

        public override int Excute(CommandArgs args)
        {
            base.Excute(args);

            if (!args.CheckRequire(this, settingsFodler: true, targetFolder:true))
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

            var zipFolderController = new ZipFolderController(settingsFolderController, settings);
            if (!zipFolderController.CheckFolder())
            {
                Console.WriteLine($"{zipFolderController.ScreenName} folder does not exist. ({zipFolderController.FolderInfo.FullName})\n");
                return -1;
            }

            var filenames = zipFolderController.GetAllFilesName();
            if(filenames == null)
            {
                Console.WriteLine($"Failed to read {zipFolderController.ScreenName} folder. ({zipFolderController.FolderInfo.FullName})\n");
                return -1;
            }

            Console.WriteLine($"Zip files: \n\t{string.Join("\n\t", filenames)}");

            return 0;
        }
    }
}
