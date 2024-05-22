using StudioIdGames.IdentifierArchiveCore.Commands;
using StudioIdGames.IdentifierArchiveCore.Files;

namespace StudioIdGames.IdentifierArchiveCore.FolderControllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetFolderPath">SettingsFile から見た ターゲットフォルダーの場所</param>
    public class TargetFolderController(SettingsFolderController settingsFolderController, string targetFolderPath) : FolderController
    {
        public PathIdentityInfo<DirectoryInfo> PathInfo => new(FolderInfo, settingsFolderController.FolderInfo);

        public override string ScreenName => "Target";

        public override DirectoryInfo FolderInfo => new($"{settingsFolderController.FolderInfo.FullName}/{targetFolderPath}");

        public override bool FolderSetup(CommandArgs args)
        {
            return base.FolderSetup(args) && IdentifierFileSetup();
        }

        private bool IdentifierFileSetup()
        {
            bool res = new IdentifierFile(IdentifierFileType.Archive).ToFile(FolderInfo, out var created, out _, autoCreate: true, autoOverwrite: false) || created;
            
            if (!res) return false;

            res = new IdentifierFile(IdentifierFileType.Archive).ToFile(FolderInfo, out created, out _, autoCreate: true, autoOverwrite: false) || created;
            
            if (!res) return false;

            res = new GitignoreFile(GitignoreFileType.Target).ToFile(FolderInfo, out created, out _, autoCreate: true, autoOverwrite: false) || created;

            if (!res) return false;

            return true;
        }
    }
}