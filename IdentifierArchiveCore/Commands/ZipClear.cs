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

            var allFiles = zipFolderController.GetAllFiles()
                ?.OrderBy(f => f.LastAccessTimeUtc)
                ?.Where(file => file.Extension.TrimStart('.') == settingsWithOutIdentifier.ZipExtension.TrimStart('.'))
                ?.ToArray() ?? [];

            if (allFiles.Length > 0)
            {
                if (allFiles.Length <= 2)
                {
                    unsafeFileNames.AddRange(allFiles.Select(file => file.FullName));
                }
                else if (allFiles.Length > 0)
                {
                    var lastAccess = allFiles.Last().LastAccessTimeUtc;
                    var saveTime = lastAccess - TimeSpan.FromDays(7);
                    Console.WriteLine($"Last access time : UTC {lastAccess} (Files before UTC {saveTime} are automatically deleted.)\n");
                    var lastSaveIndex = Array.FindIndex(allFiles, file => file.LastAccessTimeUtc >= saveTime);
                    var saveFiles = allFiles[lastSaveIndex..];

                    unsafeFileNames.AddRange(saveFiles.Select(file => file.FullName));
                }
            }

            if (string.IsNullOrWhiteSpace(args.Identifier))
            {
                int count = 0;
                foreach (var file in allFiles)
                {
                    if (zipFolderController.DeleteCache(unsafeFileNames, args.AutoFileOverwrite, Path.GetFileNameWithoutExtension(file.Name)))
                    {
                        count++;
                    }
                }

                Console.WriteLine($"Delete \"{count}\" {settingsWithOutIdentifier.ZipExtension} and meta files. ");
            }
            else
            {
                zipFolderController.DeleteCache(unsafeFileNames, args.AutoFileOverwrite, args.Identifier);
            }

            return 0;
        }
    }
}
