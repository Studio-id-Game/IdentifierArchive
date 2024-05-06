using StudioIdGames.IdentifierArchiveCore.Files;

namespace StudioIdGames.IdentifierArchiveCore
{
    public sealed class Init : ActionBase
    {
        public override string Command => Commands.INIT;

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

            var settingsFilePath = args[0];
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

            var targetName = args[1];
            var targetPath = $"{setting.TargetBasePath.TrimEnd('/', '\\')}/{targetName}";
            var targetDirectoryInfo = new DirectoryInfo(targetPath);

            if (!targetDirectoryInfo.Exists)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = $"Target Directory が見つかりません。({targetDirectoryInfo.FullName})"
                };
            }

            var identifierFileInfo = new FileInfo($"{targetDirectoryInfo.FullName}/{IdentifierFile.ArchiveFileName}");
            if (!identifierFileInfo.Exists)
            {
                File.WriteAllText(identifierFileInfo.FullName, IdentifierFile.DefaultValue);
            }

            var currentIdentifierFileInfo = new FileInfo($"{targetDirectoryInfo.FullName}/{IdentifierFile.CurrentFileName}");
            if (!currentIdentifierFileInfo.Exists)
            {
                File.WriteAllText(currentIdentifierFileInfo.FullName, IdentifierFile.DefaultValue);
            }

            var ignoreFileInfo = new FileInfo($"{targetDirectoryInfo.FullName}/{GitIgnoreFile.FileName}");
            if (!ignoreFileInfo.Exists)
            {
                File.WriteAllText(ignoreFileInfo.FullName, GitIgnoreFile.ValueInTargetDirectory);
            }

            return null;
        }
    }
}