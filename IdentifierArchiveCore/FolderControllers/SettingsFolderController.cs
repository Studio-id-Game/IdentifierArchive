using StudioIdGames.IdentifierArchiveCore.Commands;
using StudioIdGames.IdentifierArchiveCore.Files;

namespace StudioIdGames.IdentifierArchiveCore.FolderControllers
{

    public sealed class SettingsFolderController(string settingsFolderPath) : FolderController()
    {
        public override string ScreenName => "Settings";

        public override DirectoryInfo FolderInfo => new(settingsFolderPath);

        public override bool FolderSetup(CommandArgs args)
        {
            var ret = base.FolderSetup(args) && SettingsSetup(args);
            FolderSetupEnd(ret);
            return ret;
        }

        private bool SettingsSetup(CommandArgs args)
        {
            return new SettingsFile().ToFile(FolderInfo, out var created, out var overwrited, autoCreate: true, autoOverwrite: args.AutoFileOverwrite) || created || overwrited;
        }

        public SettingsFile? GetSettingsFile()
        {
            if (!CheckFolder())
            {
                return null;
            }

            var file = new SettingsFile().FromFile(FolderInfo);

            return file?.GetReplaced(FolderInfo);
        }
    }
}