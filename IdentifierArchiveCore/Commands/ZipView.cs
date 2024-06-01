using StudioIdGames.IdentifierArchiveCore.Files;
using StudioIdGames.IdentifierArchiveCore.FolderControllers;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class ZipView : CommandAction
    {
        public static ZipView Instance { get; } = new ZipView();

        private ZipView() { }

        public override string CommandID => "zv";

        public override string Name => "Zip-View";

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

            var zipFolderController = new ZipFolderController(settings);
            if (!zipFolderController.CheckFolder())
            {
                Console.WriteLine($"{zipFolderController.ScreenName} folder does not exist. ({zipFolderController.FolderInfo.FullName})\n");
                return -1;
            }

            var allFiles = zipFolderController.GetAllFiles();

            if(allFiles == null)
            {
                Console.WriteLine($"Failed to read {zipFolderController.ScreenName} folder. ({zipFolderController.FolderInfo.FullName})\n");
                return -1;
            }

            Console.WriteLine($"Zip files:");
            foreach ( var file in allFiles)
            {
                if(file.Extension.TrimStart('.') == settings.ZipExtension.TrimStart('.'))
                {
                    UserInfoFile? meta;
                    using(new ConsoleOutKill())
                    {
                        meta = zipFolderController.GetZipMetaFile(Path.GetFileNameWithoutExtension(file.Name));
                    }

                    Console.WriteLine($"\t{zipFolderController.GetRelativePath(file)}\t owner: {meta?.UserName ?? "??"} ({meta?.UserID ?? "??"})");
                }
            }


            return 0;
        }
    }
}
