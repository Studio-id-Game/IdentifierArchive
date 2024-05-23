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

        /// <summary>
        /// 設定ファイルを読み込みます。
        /// </summary>
        /// <returns></returns>
        public SettingsFile? GetSettingsFile(DirectoryInfo? targetFolderInfo = null, string? identifier = null, bool loadLocalkey = true)
        {
            return GetSettingsFile(out _, out _, targetFolderInfo: targetFolderInfo, identifier: identifier, loadLocalkey: loadLocalkey);
        }

        /// <summary>
        /// 設定ファイルを読み込みます。
        /// </summary>
        /// <returns></returns>
        public SettingsFile? GetSettingsFile(out SettingsFile? rawView, out SettingsFile? safeView, DirectoryInfo? targetFolderInfo = null, string? identifier = null, bool loadLocalkey = true)
        {
            if (!CheckFolder())
            {
                rawView = null;
                safeView = null;    
                return null;
            }

            rawView = new SettingsFile().FromFile(FolderInfo);
            if (rawView == null) 
            {
                safeView = null;
                return null; 
            }

            var file = new SettingsFile();
            file.CopyFrom(rawView);
            file.Replace(out safeView, settingsFolderInfo: FolderInfo, targetFolderInfo: targetFolderInfo, identifier: identifier, loadLocalkey: loadLocalkey);

            return file;
        }
    }
}