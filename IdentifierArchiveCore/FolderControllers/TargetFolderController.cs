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
            var ret = base.FolderSetup(args) && IdentifierFileSetup(args);
            FolderSetupEnd(ret);
            return ret;
        }

        private bool IdentifierFileSetup(CommandArgs args)
        {
            var archive = GetArchiveIdentifier(false)!;
            var current = GetCurrentIdentifier(false)!;
            var gitignore = GetGitignore(false)!;

            if (!string.IsNullOrWhiteSpace(args.Identifier))
            {
                archive.Text = args.Identifier;
                current.Text = args.Identifier;
            }

            archive.FixIdentifier(0);
            current.FixIdentifier(0);

            bool exists = archive.ToFile(FolderInfo, out var created, out _, autoCreate: true, autoOverwrite: args.AutoFileOverwrite);

            if (!exists && !created) return false;

            exists = current.ToFile(FolderInfo, out created, out _, autoCreate: true, autoOverwrite: args.AutoFileOverwrite);
            
            if (!exists && !created) return false;

            exists = gitignore.ToFile(FolderInfo, out created, out _, autoCreate: true, autoOverwrite: args.AutoFileOverwrite);

            if (!exists && !created) return false;

            return true;
        }

        public IdentifierFile? GetArchiveIdentifier(bool loading)
        {
            var archive = new IdentifierFile(IdentifierFileType.Archive);
            return loading ? archive.FromFile(FolderInfo) : archive;
        }

        public IdentifierFile? GetCurrentIdentifier(bool loading)
        {
            var current = new IdentifierFile(IdentifierFileType.Current);
            return loading ? current.FromFile(FolderInfo) : current;
        }

        public GitignoreFile? GetGitignore(bool loading)
        {
            var gitignore = new GitignoreFile(GitignoreFileType.Target);
            return loading ? gitignore.FromFile(FolderInfo) : gitignore;
        }

        public bool SetIdentifier(string identifier)
        {
            var archive = new IdentifierFile(IdentifierFileType.Archive)
            {
                Text = identifier
            };

            var current = new IdentifierFile(IdentifierFileType.Current)
            {
                Text = identifier
            };

            return archive.CreateOrUpdate(FolderInfo) && current.CreateOrUpdate(FolderInfo);
        }

        public bool SetCurrentIdentifier(string identifier)
        {
            var current = new IdentifierFile(IdentifierFileType.Current)
            {
                Text = identifier
            };

            return current.CreateOrUpdate(FolderInfo);
        }
    }
}