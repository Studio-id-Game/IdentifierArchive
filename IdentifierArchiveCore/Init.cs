using StudioIdGames.IdentifierArchiveCore.Files;

namespace StudioIdGames.IdentifierArchiveCore
{
    public sealed class Init : ActionBase
    {
        public override string Command => Commands.INIT;

        public override ActionInfo? Excute(ReadOnlySpan<string> args)
        {
            if (args.Length < 1)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = "引数の数が足りません"
                };
            }

            var directoryPath = args[0];
            var directoryInfo = new DirectoryInfo(directoryPath);

            var identifierFileInfo = new FileInfo($"{directoryInfo.FullName}/{IdentifierFile.ArchiveFileName}");
            if (!identifierFileInfo.Exists)
            {
                File.WriteAllText(identifierFileInfo.FullName, IdentifierFile.DefaultValue);
            }

            var currentIdentifierFileInfo = new FileInfo($"{directoryInfo.FullName}/{IdentifierFile.CurrentFileName}");
            if (!currentIdentifierFileInfo.Exists)
            {
                File.WriteAllText(currentIdentifierFileInfo.FullName, IdentifierFile.DefaultValue);
            }

            var ignoreFileInfo = new FileInfo($"{directoryInfo.FullName}/{GitIgnoreFile.FileName}");
            if (!ignoreFileInfo.Exists)
            {
                File.WriteAllText(ignoreFileInfo.FullName, GitIgnoreFile.ValueInTargetDirectory);
            }

            return null;
        }
    }
}