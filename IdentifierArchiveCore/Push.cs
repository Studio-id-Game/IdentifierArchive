using StudioIdGames.IdentifierArchiveCore.Files;
using System.Diagnostics;
using System.IO;

namespace StudioIdGames.IdentifierArchiveCore
{
    public sealed class Push : ActionBase
    {
        public override string Command => Commands.PUSH;

        public override ActionInfo? Excute(ReadOnlySpan<string> args)
        {
            var settingsFilePath = args[0];
            var targetName = args[1];
            var customIdentifier = args.Length > 2 ? Utility.FixIdentifier(args[2], 0) : null;
            var initRes = new Init().Excute(args);

            if (initRes != null && initRes.IsError)
            {
                return initRes;
            }


            if (args.Length < 2)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = "引数の数が足りません"
                };
            }

            var settingFileInfo = new FileInfo($"{settingsFilePath}/{SettingsFile.FileName}");
            if (!settingFileInfo.Exists)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = $"Settings File が見つかりません。({settingFileInfo.FullName})"
                };
            }

            var setting = SettingsFile.FromBytes(File.ReadAllBytes(settingFileInfo.FullName));
            var targetPath = $"{setting.TargetBasePath.TrimEnd('/', '\\')}/{targetName}";

            var localKeyFileInfo = new FileInfo($"{settingsFilePath}/{LocalKeyFile.FileName}");
            var targetDirectoryInfo = new DirectoryInfo(targetPath);
            var identifierFileInfo = new FileInfo($"{targetDirectoryInfo.FullName}/{IdentifierFile.ArchiveFileName}");
            var currentIdentifierFileInfo = new FileInfo($"{targetDirectoryInfo.FullName}/{IdentifierFile.CurrentFileName}");

            var newIdentifier = customIdentifier ?? Utility.FixIdentifier(File.ReadAllText(identifierFileInfo.FullName), 1);
            LocalKeyFile.Data? localKey = null;
            if (localKeyFileInfo.Exists)
            {
                localKey = LocalKeyFile.FromBytes(File.ReadAllBytes(localKeyFileInfo.FullName));
            }

            setting.SetEnvironment(
                targetName: targetName,
                identifier: newIdentifier,
                settingsPath: $"{settingFileInfo.Directory!.FullName}/{Path.GetFileNameWithoutExtension(settingFileInfo.FullName)}",
                localKey: localKey);

            var zipFileInfo = new FileInfo(setting.ZipPath);
            var zipFileFolderDirectoryInfo = zipFileInfo.Directory!;
            if (!zipFileFolderDirectoryInfo.Exists)
            {
                zipFileFolderDirectoryInfo.Create();
            }

            var zipIgnoreFileInfo = new FileInfo(zipFileFolderDirectoryInfo.FullName + "/.gitignore");
            if (!zipIgnoreFileInfo.Exists)
            {
                File.WriteAllText(
                zipIgnoreFileInfo.FullName, "*" + zipFileInfo.Extension);
            }

            var resZip = Utility.ExcuteCommand(setting.ZipCommand);

            if(resZip != 0)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = "Zip command is bad."
                };
            }

            Console.WriteLine("Zip command is completed.");

            var resUpload = Utility.ExcuteCommand(setting.UploadCommand);

            if (resUpload != 0)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = "Upload command is bad."
                };
            }

            Console.WriteLine("Upload command is completed.");

            File.WriteAllText(identifierFileInfo.FullName, newIdentifier);
            File.WriteAllText(currentIdentifierFileInfo.FullName, newIdentifier);

            Console.WriteLine("Identifier file is updated.");

            return null;
        }

    }
}