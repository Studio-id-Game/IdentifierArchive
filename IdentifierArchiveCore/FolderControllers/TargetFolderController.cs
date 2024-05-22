﻿using StudioIdGames.IdentifierArchiveCore.Commands;
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
            return base.FolderSetup(args) && IdentifierFileSetup(args);
        }

        private bool IdentifierFileSetup(CommandArgs args)
        {
            var archive = GetArchiveIdentifier(false)!;
            var current = GetCurrentIdentifier(false)!;

            if (!string.IsNullOrWhiteSpace(args.Identifier))
            {
                archive.Text = args.Identifier;
                current.Text = args.Identifier;
            }

            bool res = archive.ToFile(FolderInfo, out var created, out _, autoCreate: true, autoOverwrite: false) || created;
            
            if (!res) return false;

            res = current.ToFile(FolderInfo, out created, out _, autoCreate: true, autoOverwrite: false) || created;
            
            if (!res) return false;

            res = new GitignoreFile(GitignoreFileType.Target).ToFile(FolderInfo, out created, out _, autoCreate: true, autoOverwrite: false) || created;

            if (!res) return false;

            return true;
        }

        public IdentifierFile? GetArchiveIdentifier(bool loading)
        {
            var archive = new IdentifierFile(IdentifierFileType.Archive);
            return loading ? archive.FromFile(FolderInfo) : archive;
        }

        public IdentifierFile? GetCurrentIdentifier(bool loading)
        {
            var archive = new IdentifierFile(IdentifierFileType.Current);
            return loading ? archive.FromFile(FolderInfo) : archive;
        }
    }
}