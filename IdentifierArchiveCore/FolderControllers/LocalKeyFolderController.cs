using StudioIdGames.IdentifierArchiveCore.Commands;
using StudioIdGames.IdentifierArchiveCore.Files;

namespace StudioIdGames.IdentifierArchiveCore.FolderControllers
{
    public sealed class LocalKeyFolderController(SettingsFile settings) : FolderController
    {
        public override string ScreenName => "LocalKey";

        public override DirectoryInfo FolderInfo => settings.GetLocalkeyFolderInfo();

        public override bool FolderSetup(CommandArgs args)
        {
            return base.FolderSetup(args) && LocalkeySetup();
        }

        private bool LocalkeySetup()
        {
            return new LocalKeyFile().ToFile(FolderInfo, out var created, out _, autoCreate: true, autoOverwrite: false) || created;
        }

        public LocalKeyFile? GetLocalKey()
        {
            return new LocalKeyFile().FromFile(FolderInfo);
        }

        public string[]? GetLocalKeyFileNames()
        {
            if (!CheckFolder())
            {
                return null;
            }

            var localkeyFiles = FolderInfo.GetFiles("*", SearchOption.AllDirectories);
            return localkeyFiles?.Select(e => Path.GetRelativePath(settings.LocalkeyFolderAbsolute, e.FullName)).ToArray();
        }
    }
}