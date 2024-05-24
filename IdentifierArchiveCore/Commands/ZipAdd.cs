using StudioIdGames.IdentifierArchiveCore.Files;
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

            var settingsFolderController = new SettingsFolderController(args.SettingsFolder);
            var targetFolderController = new TargetFolderController(settingsFolderController, args.TargetFolder);
            var archiveIdentifier = targetFolderController.GetArchiveIdentifier(true) ?? new IdentifierFile(IdentifierFileType.Archive);

            if (!string.IsNullOrWhiteSpace(args.Identifier))
            {
                archiveIdentifier.Text = args.Identifier;
                archiveIdentifier.FixIdentifier(0);
            }

            var settings = settingsFolderController.GetSettingsFile(out _, out var safe, targetFolderController.FolderInfo, archiveIdentifier.Text);

            if (settings == null)
            {
                return -1;
            }

            var zipFileInfo = settings.GetZipFileInfo();
            
            while (zipFileInfo.Exists)
            {
                Console.WriteLine($"{archiveIdentifier.Text} exists. ({zipFileInfo.FullName})");
                archiveIdentifier.FixIdentifier(1);

                settings = settingsFolderController.GetSettingsFile(out _, out safe, targetFolderController.FolderInfo, archiveIdentifier.Text);
                
                if (settings == null)
                {
                    return -1;
                }

                zipFileInfo = settings.GetZipFileInfo();
            }

            Console.WriteLine($"Create zip file. ({zipFileInfo.FullName})");

            Console.WriteLine($"Excute Zip Command :\n{safe?.ZipCommand}\n");

            var ret = settings.ExcuteZip();

            Console.WriteLine();

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

            Console.WriteLine("Updated Identifier files.\n");

            return 0;
        }
    }
}
