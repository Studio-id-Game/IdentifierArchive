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

            var settingsWithOutIdentifier = settingsFolderController.GetSettingsFile(out _, out var safe, targetFolderController.FolderInfo);

            if (settingsWithOutIdentifier == null)
            {
                return -1;
            }

            var backupZipFolderController = new ZipFolderController(settingsWithOutIdentifier, ZipFolderController.BackupIdentifier);
            var unsafeFileNames = new List<string>(){
                  backupZipFolderController.GetZipFileInfo().FullName,
            };

            var currentIdentifier = targetFolderController.GetCurrentIdentifier(true)?.Text;
            if (currentIdentifier != null)
            {
                unsafeFileNames.Add(new ZipFolderController(settingsWithOutIdentifier, currentIdentifier).GetZipFileInfo().FullName);
            }

            var archiveIdentifier = targetFolderController.GetArchiveIdentifier(true)?.Text;
            if (archiveIdentifier != null)
            {
                unsafeFileNames.Add(new ZipFolderController(settingsWithOutIdentifier, archiveIdentifier).GetZipFileInfo().FullName);
            }

            var allFiles = backupZipFolderController.GetAllFiles()?.ToArray() ?? [];
            if(allFiles.Length > 0)
            {
                if (allFiles.Length <= 2)
                {
                    unsafeFileNames.AddRange(allFiles.Select(file => file.FullName));
                }
                else if (allFiles.Length > 0)
                {
                    var lastAccess = allFiles.MaxBy(file => file.LastAccessTimeUtc)?.LastAccessTimeUtc ?? DateTime.UtcNow;
                    var lastests = (backupZipFolderController.GetAllFiles() ?? [])
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
                var zipFolderController = new ZipFolderController(settingsWithOutIdentifier, args.Identifier);
                zipFolderController.DeleteCache(unsafeFileNames, args.AutoFileOverwrite);
            }

            return 0;
        }
    }
}
