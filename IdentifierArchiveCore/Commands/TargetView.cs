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

            var settingsFolderController = new SettingsFolderController(args.SettingsFolder!);
            var targetFolderController = new TargetFolderController(settingsFolderController, args.TargetFolder!);
            var settings = settingsFolderController.GetSettingsFile()
                ?.GetReplaced(targetFolderInfo: targetFolderController.FolderInfo, identifier: args.Identifier);

            if (settings == null)
            {
                return -1;
            }

            var archiveIdentifier = targetFolderController.GetArchiveIdentifier(true)?.Text ?? "(null)";
            var currentIdentifier = targetFolderController.GetCurrentIdentifier(true)?.Text ?? "(null)";

            Console.WriteLine($"Archive Identifier: {archiveIdentifier}\n");
            Console.WriteLine($"Current Identifier: {currentIdentifier}\n");

            targetFolderController.GitFileNameList(settings.GitExePath, out var fileNames, out var ignoreFileNames, false);

            foreach (var item in ignoreFileNames ?? [])
            {
                Console.WriteLine($"\tArchive File: ... {args.TargetFolder}/{item}");
            }

            foreach (var item in fileNames ?? [])
            {
                Console.WriteLine($"\tGit File: ... {args.TargetFolder}/{item}");
            }

            Console.WriteLine();

            return 0;
        }
    }
}
