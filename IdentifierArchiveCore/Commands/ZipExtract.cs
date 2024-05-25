using StudioIdGames.IdentifierArchiveCore.Files;
using StudioIdGames.IdentifierArchiveCore.FolderControllers;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class ZipExtract : CommandAction
    {
        public static ZipExtract Instance { get; } = new ZipExtract();

        private ZipExtract() { }

        public override string CommandID => "zx";

        public override string Name => "Zip-Extract";



        public override int Excute(CommandArgs args)
        {
            base.Excute(args);

            if (!args.CheckRequire(this, settingsFodler: true, targetFolder: true))
            {
                return -1;
            }

            var settingsFolderController = new SettingsFolderController(args.SettingsFolder!);
            var targetFolderController = new TargetFolderController(settingsFolderController, args.TargetFolder!);
            var identifier = string.IsNullOrWhiteSpace(args.Identifier) ? targetFolderController.GetArchiveIdentifier(true)?.Text : args.Identifier;

            if (identifier == null)
            {
                Console.WriteLine("Not found ArchiveIdentifier file of not set -id augument.\n");
                return -1;
            }

            if (identifier == targetFolderController.GetCurrentIdentifier(true)?.Text)
            {
                Console.WriteLine("Current and Archive identifiers match.");
                if (!ConsoleUtility.Question("Do you want to cancel the current edit and revert to the archived state? (Current edits will be backed up)", args.AutoFileOverwrite))
                {
                    Console.WriteLine("Not revert to the archived state.\n");
                    return 0;
                }
                else
                {
                    Console.WriteLine("Revert to the archived state.\n");
                }
            }

            var settings = settingsFolderController.GetSettingsFile(out _, out var settingsSafe, targetFolderController.FolderInfo, identifier);

            if (settings == null || settingsSafe == null)
            {
                return -1;
            }

            var zipFileInfo = settings.GetZipFileInfo();

            if (!zipFileInfo.Exists)
            {
                Console.WriteLine($"Zip file does no exist. ({zipFileInfo.FullName})");
                return -1;
            }

            if (identifier != ZipFolderController.BackupIdentifier)
            {
                var backupSettings = settingsFolderController.GetSettingsFile(out _, out var backupSettingsSafe, targetFolderController.FolderInfo, ZipFolderController.BackupIdentifier);

                if (backupSettings == null || backupSettingsSafe == null)
                {
                    return -1;
                }

                Console.WriteLine($"Backup old files ({backupSettingsSafe.GetZipFileInfo().FullName})");

                var backupFileinfo = backupSettings.GetZipFileInfo();
                if (backupFileinfo.Exists)
                {
                    backupFileinfo.Delete();
                }

                Console.WriteLine($"Excute Backup Command :\n{backupSettingsSafe.ZipCommand}\n");

                var backupExit = backupSettings.ExcuteZip();

                Console.WriteLine();

                if (backupExit < 0)
                {
                    Console.WriteLine("Backup Command failed.\n");
                    return backupExit;
                }
                else
                {
                    Console.WriteLine("Backup Command complete.\n");
                }
            }

            targetFolderController.GitFileList(settings.GitExePath, out var _, out var ignoredFiles);

            Console.WriteLine($"Delete old files\n");

            foreach (var item in ignoredFiles)
            {
                var relativePath = Path.GetRelativePath(targetFolderController.FolderInfo.FullName, item.FullName);
                Console.WriteLine($"\tDelete: {relativePath}");
                item.Delete();
            }

            Console.WriteLine();

            Console.WriteLine($"Excute Unzip Command :\n{settingsSafe.UnzipCommand}\n");

            var unzipExit = settings.ExcuteUnzip();

            Console.WriteLine();

            if (unzipExit < 0)
            {
                Console.WriteLine("Unzip Command failed.\n");
                return unzipExit;
            }
            else
            {
                Console.WriteLine("Unzip Command complete.\n");
            }


            Console.WriteLine("Update Identifier files...\n");

            if (!targetFolderController.SetCurrentIdentifier(identifier))
            {
                return -1;
            }

            Console.WriteLine($"Updated current Identifier file. ({identifier})\n");

            return 0;
        }
    }
}
