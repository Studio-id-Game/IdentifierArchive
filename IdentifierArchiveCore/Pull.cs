using StudioIdGames.IdentifierArchiveCore.Files;

namespace StudioIdGames.IdentifierArchiveCore
{
    public sealed class Pull : ActionBase
    {
        public override string Command => Commands.PULL;

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

            controller.SetupTargetFolder();

            var newIdentifier = customIdentifier ?? TextFile.FromFile(controller.IdentifierFileInfo);

            if (newIdentifier == null)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = "Identifier is null."
                };
            }

            if (!controller.TryLoad(newIdentifier))
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = "Can not load settings file."
                };
            }

            controller.SetupZipFolder();

            Console.WriteLine("\nInitialized folders.\n");

            if (!controller.ZipFileInfo.Exists)
            {
                Console.WriteLine("Excute download command.\n");
                var resDownload = controller.ExcuteDownload();
                Console.WriteLine("\nDownload command is completed.\n");

                if (resDownload != 0)
                {
                    return new ActionInfo()
                    {
                        IsError = true,
                        Message = "Download command is bad."
                    };
                }
            }

            Console.WriteLine("Create backup files.\n");
            var files = controller.TargetFolderInfo.Info.GetFiles("*", SearchOption.AllDirectories);
            var backupFolderInfo = new DirectoryInfo($"{controller.TargetFolderInfo.Info.FullName}/IdentifierArchiveBackup~");
            if (backupFolderInfo.Exists)
            {
                if (backupFolderInfo.GetFiles("*", SearchOption.AllDirectories).Length > 0)
                {
                    return new ActionInfo()
                    {
                        IsError = true,
                        Message = $"There is backup files, check the fodler : {backupFolderInfo.FullName}."
                    };
                }
            }
            else
            {
                backupFolderInfo.Create();
            }

            foreach (var file in files)
            {
                if (file.Extension != IdentifierFile.Extension && file.Name != GitIgnoreFile.FileName)
                {
                    var relativePath = Path.GetRelativePath(controller.TargetFolderInfo.Info.FullName, file.FullName);
                    var fileInfo = new FileInfo($"{backupFolderInfo.FullName}/{relativePath}");
                    Console.WriteLine($"Move to {fileInfo.FullName}");
                    var folder = fileInfo.Directory;
                    if (folder!= null && !folder.Exists)
                    {
                        folder.Create();
                    }
                    file.MoveTo(fileInfo.FullName);
                }
            }

            Console.WriteLine("Excute unzip command.\n");
            var resUnzip = controller.ExcuteUnzip();
            Console.WriteLine();

            if (resUnzip != 0)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = "Unzip command is bad."
                };
            }

            backupFolderInfo.Delete(true);


            Console.WriteLine("Unzip command is completed.\n");

            TextFile.ToFile(controller.CurrentIdentifierFileInfo, newIdentifier);

            Console.WriteLine("Identifier file is updated.\n");
            return null;
        }
    }
}