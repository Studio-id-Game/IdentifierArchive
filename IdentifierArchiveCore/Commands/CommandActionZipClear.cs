using StudioIdGames.IdentifierArchiveCore.Files;
using StudioIdGames.IdentifierArchiveCore.FolderControllers;
using System.Collections.Generic;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandActionZipClear : CommandAction
    {
        public static CommandActionZipClear Instance { get; } = new CommandActionZipClear();

        private CommandActionZipClear() { }

        public override string CommandID => "zc";

        public override string Name => "Zip-Clear";

        public override int Excute(CommandArgs args)
        {
            base.Excute(args);

            if (!args.CheckRequire(this, settingsFodler: true, targetFolder: true))
            {
                return -1;
            }

            var settingsFolderController = new SettingsFolderController(args.SettingsFolder!);
            var targetFolderController = new TargetFolderController(settingsFolderController, args.TargetFolder!);

            var settingsWithOutIdentifier = settingsFolderController.GetSettingsFile()
                ?.GetReplaced(targetFolderInfo: targetFolderController.FolderInfo);

            if (settingsWithOutIdentifier == null)
            {
                return -1;
            }

            var zipFolderController = new ZipFolderController(settingsWithOutIdentifier);

            var unsafeFileNames = new List<string>(){
                  zipFolderController.GetZipFileInfo(ZipFolderController.BackupIdentifier).FullName,
            };

            var currentIdentifier = targetFolderController.GetCurrentIdentifier(true)?.Text;
            if (currentIdentifier != null)
            {
                unsafeFileNames.Add(zipFolderController.GetZipFileInfo(currentIdentifier).FullName);
            }

            var archiveIdentifier = targetFolderController.GetArchiveIdentifier(true)?.Text;
            if (archiveIdentifier != null)
            {
                unsafeFileNames.Add(zipFolderController.GetZipFileInfo(archiveIdentifier).FullName);
            }

            zipFolderController.DeleteZipFile(unsafeFileNames, args.AutoFileOverwrite, args.Identifier);

            return 0;
        }
    }
}
