using StudioIdGames.IdentifierArchiveCore.Files;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace StudioIdGames.IdentifierArchiveCore
{
    public sealed class Push : ActionBase
    {
        public override string Command => Commands.PUSH;

        public override ActionInfo? Excute(ReadOnlySpan<string> args)
        {
            if (args.Length < 2)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = "引数の数が足りません"
                };
            }

            var settingsFolderPath = args[0];
            var targetFolderPath = args[1];
            var customIdentifier = args.Length > 2 ? Utility.FixIdentifier(args[2], 0) : null;

            TargetFolderController controller = new(settingsFolderPath, targetFolderPath);

            var fileCheck = controller.CheckSettingsFileAndTargetFolder();
            if (fileCheck != null)
            {
                return fileCheck;
            }

            controller.CreateInitFiles();

            var newIdentifier = customIdentifier ?? Utility.FixIdentifier(TextFile.FromFile(controller.IdentifierFileInfo)!, 1);


            if (controller.TryLoad(newIdentifier))
            {
                if (!controller.ZipFolderInfo.Exists)
                {
                    controller.ZipFolderInfo.Create();
                }

            }

            var zipIgnoreFileInfo = new FileInfo($"{controller.ZipFolderInfo.FullName}/{GitIgnoreFile.FileName}");
            if (!zipIgnoreFileInfo.Exists)
            {
                TextFile.ToFile(zipIgnoreFileInfo, $"*{zipIgnoreFileInfo.Extension}");
            }

            Console.WriteLine();
            Console.WriteLine("Initialized folders.");
            Console.WriteLine();
            Console.WriteLine("Excute zip command.\n");
            var resZip = controller.ExcuteZip();
            Console.WriteLine();

            if (resZip != 0)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = "Zip command is bad."
                };
            }

            Console.WriteLine("Zip command is completed.\n");

            Console.WriteLine("\nExcute upload command.\n");
            var resUpload = controller.ExcuteUpload();
            Console.WriteLine();


            if (resUpload != 0)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = "Upload command is bad."
                };
            }

            Console.WriteLine("Upload command is completed.\n");

            TextFile.ToFile(controller.IdentifierFileInfo, newIdentifier);
            TextFile.ToFile(controller.CurrentIdentifierFileInfo, newIdentifier);

            Console.WriteLine("Identifier file is updated.\n");

            return null;
        }

    }
}