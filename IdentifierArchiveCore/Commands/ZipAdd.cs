﻿using StudioIdGames.IdentifierArchiveCore.Files;
using StudioIdGames.IdentifierArchiveCore.FolderControllers;
using System.IO;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class ZipAdd : CommandAction
    {
        public static ZipAdd Instance { get; } = new ZipAdd();

        private ZipAdd() { }

        public override string CommandID => "za";

        public override string Name => "Zip-Add";

        public override int Excute(CommandArgs args)
        {
            base.Excute(args);

            if (!args.CheckRequire(this, settingsFodler: true, targetFolder: true))
            {
                return -1;
            }

            var settingsFolderController = new SettingsFolderController(args.SettingsFolder!);
            var targetFolderController = new TargetFolderController(settingsFolderController, args.TargetFolder!);
            var archiveIdentifier = targetFolderController.GetArchiveIdentifier(true) ?? new IdentifierFile(IdentifierFileType.Archive);

            if (string.IsNullOrWhiteSpace(args.Identifier))
            {
                archiveIdentifier.FixIdentifier(1);
            }
            else
            {
                archiveIdentifier.Text = args.Identifier;
                archiveIdentifier.FixIdentifier(0);
            }

            var settingsWithOutIdentifier = settingsFolderController.GetSettingsFile()?
                .GetReplaced(targetFolderInfo: targetFolderController.FolderInfo);

            if (settingsWithOutIdentifier == null)
            {
                return -1;
            }

            var settings = settingsWithOutIdentifier.GetReplaced(identifier: archiveIdentifier.Text);

            var zipFileInfo = settings.GetZipFileInfo();
            if (zipFileInfo.Exists)
            {
                Console.WriteLine($"File is exists. ({zipFileInfo.FullName})");

                if (ConsoleUtility.Question("Overwrite this file?", args.AutoFileOverwrite))
                {
                    ConsoleUtility.DeleteFile(zipFileInfo, [], true);
                }
                else 
                {
                    var oldIdentifier = archiveIdentifier.Text;
                    if (settingsWithOutIdentifier == null)
                    {
                        return -1;
                    }

                    while (zipFileInfo.Exists)
                    {
                        archiveIdentifier.FixIdentifier(1);
                        settings = settingsWithOutIdentifier.GetReplaced(identifier: archiveIdentifier.Text);
                        zipFileInfo = settings.GetZipFileInfo();
                    }

                    if (!ConsoleUtility.Question($"Increment identifier? ({oldIdentifier} to {archiveIdentifier.Text})", args.AutoIdentifierIncrement))
                    {
                        return -1;
                    }
                }
            }

            Console.WriteLine($"Create zip file. ({zipFileInfo.FullName})");

            var ret = settings.ExcuteZip();

            if(ret < 0)
            {
                Console.WriteLine("Zip Command failed.\n");
                return ret;
            }

            Console.WriteLine("Zip Command complete.\n");

            Console.WriteLine("Update Identifier files...\n");

            if (!targetFolderController.SetIdentifier(archiveIdentifier.Text))
            {
                return -1;
            }

            Console.WriteLine($"Updated Identifier files. ({archiveIdentifier.Text})\n");

            return 0;
        }
    }
}
