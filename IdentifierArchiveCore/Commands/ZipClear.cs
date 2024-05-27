using StudioIdGames.IdentifierArchiveCore.Files;
using StudioIdGames.IdentifierArchiveCore.FolderControllers;
using System.Collections.Generic;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class ZipClear : CommandAction
    {
        public static ZipClear Instance { get; } = new ZipClear();

        private ZipClear() { }

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

            var allFiles = zipFolderController.GetAllFiles()?.ToArray() ?? [];
            if(allFiles.Length > 0)
            {
                if (allFiles.Length <= 2)
                {
                    unsafeFileNames.AddRange(allFiles.Select(file => file.FullName));
                }
                else if (allFiles.Length > 0)
                {
                    var lastAccess = allFiles.MaxBy(file => file.LastAccessTimeUtc)?.LastAccessTimeUtc ?? DateTime.UtcNow;
                    var lastests = (zipFolderController.GetAllFiles() ?? [])
                            .OrderBy(file => file.LastAccessTimeUtc)
                            .TakeLast(3);

                    Console.WriteLine(lastAccess);

                    unsafeFileNames.AddRange(lastests.Select(file => file.FullName));
                }
            }

            Console.WriteLine(string.Join(Environment.NewLine, unsafeFileNames));
            if (string.IsNullOrWhiteSpace(args.Identifier))
            {
                foreach(var file in allFiles) 
                {
                    ConsoleUtility.DeleteFile(file, unsafeFileNames, args.AutoFileOverwrite);
                }
            }
            else
            {
                zipFolderController.DeleteCache(unsafeFileNames, args.AutoFileOverwrite, args.Identifier);
            }

            return 0;
        }
    }
}
