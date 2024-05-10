﻿namespace StudioIdGames.IdentifierArchiveCore.Files
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="settingsFolderPath">実行ファイルから見た SettingsFile のあるフォルダーの場所</param>
    /// <param name="targetFolderPath">SettingsFile から見た ターゲットフォルダーの場所</param>
    public class TargetFolderController(string settingsFolderPath, string targetFolderPath)
    {
        /// <summary>
        /// 実行ファイルから見た SettingsFile の場所
        /// </summary>
        public string SettingsFolderPath { get; } = settingsFolderPath;

        /// <summary>
        /// SettingsFile から見た ターゲットフォルダーの場所
        /// </summary>
        public string TargetFolderPath { get; } = targetFolderPath;

        public SettingsFile Settings { get; private set; } = new();

        public DirectoryInfo SettingsFolderInfo => new(SettingsFolderPath);

        public FileInfo SettingsFileInfo => new($"{SettingsFolderPath}/{SettingsFile.FileName}");

        public DirectoryInfo LocalkeyFolderInfo => new(Settings.LocalkeyFolderAbsolute);

        public FileInfo LocalkeyFileInfo => new($"{Settings.LocalkeyFolderAbsolute}/{LocalKeyFile.FileName}");

        public FileInfo ZipFileInfo => new(Settings.ZipFile);
        public DirectoryInfo ZipFolderInfo => ZipFileInfo.Directory!;

        public PathIdentityInfo<DirectoryInfo> TargetFolderInfo => new(new($"{SettingsFolderPath}/{TargetFolderPath}"), SettingsFolderInfo);

        public FileInfo IdentifierFileInfo => new($"{SettingsFolderPath}/{TargetFolderPath}/{IdentifierFile.ArchiveFileName}");

        public FileInfo CurrentIdentifierFileInfo => new($"{SettingsFolderPath}/{TargetFolderPath}/{IdentifierFile.CurrentFileName}");

        public FileInfo TargetGitIgnoreFileInfo => new($"{SettingsFolderPath}/{TargetFolderPath}/{GitIgnoreFile.FileName}");

        public ActionInfo? CheckSettingsFileAndTargetFolder()
        {
            if (!SettingsFileInfo.Exists)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = $"Settings File が見つかりません。({SettingsFileInfo.FullName})"
                };
            }

            if (!TargetFolderInfo.Info.Exists)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = $"Target Folder が見つかりません。({TargetFolderInfo.Info.FullName})"
                };
            }

            return null;
        }

        public void CreateInitFiles()
        {
            if (!IdentifierFileInfo.Exists)
            {
                IdentifierFile.ToFile(IdentifierFileInfo);
            }

            if (!CurrentIdentifierFileInfo.Exists)
            {
                IdentifierFile.ToFile(CurrentIdentifierFileInfo);
            }

            if (!TargetGitIgnoreFileInfo.Exists)
            {
                GitIgnoreFile.ToFileInTargetFolder(TargetGitIgnoreFileInfo);
            }
        }

        /// <summary>
        /// 設定ファイルを読み込み、可能であれば環境変数を適用します。
        /// </summary>
        /// <param name="targetFolderPath">SettingsFile から見た ターゲットフォルダーの場所</param>
        /// <param name="identifier">ターゲットとする識別子</param>
        /// <returns></returns>
        public bool TryLoad(string identifier, bool ignoreLocalkey = false)
        {
            bool res = false;
            var settings = SettingsFile.FromFile(SettingsFileInfo);
            if (settings != null)
            {
                Settings = settings;
                res = true;
            }

            Settings.Replace(SettingsFile.IDENTIFIER, identifier);
            Settings.Replace(SettingsFile.TARGET_FOLDER, TargetFolderInfo);
            Settings.Replace(SettingsFile.SETTINGS_FOLDER_ABS, SettingsFolderInfo.FullName);
            Settings.Replace(SettingsFile.SETTINGS_FILE_ABS, SettingsFileInfo.FullName);
            Settings.Replace(SettingsFile.ZIP_FILE_ABS, ZipFileInfo.FullName);
            Settings.Replace(SettingsFile.ZIP_FOLDER_ABS, ZipFolderInfo.FullName);
            Settings.Replace(SettingsFile.LOCALKEY_FOLDER_ABS, LocalkeyFolderInfo.FullName);

            if (!ignoreLocalkey)
            {
                var localkeyFile = LocalKeyFile.FromFile(LocalkeyFileInfo);
                if (localkeyFile != null)
                {
                    foreach (var item in localkeyFile.KeySet)
                    {
                        Settings.Replace(item.Key, item.Value);
                    }
                }
            }

            return res;
        }

        public int ExcuteZip() => Utility.ExcuteCommand(Settings.ZipCommand);

        public int ExcuteUnzip() => Utility.ExcuteCommand(Settings.UnzipCommand);

        public int ExcuteUpload() => Utility.ExcuteCommand(Settings.UploadCommand);

        public int ExcuteDownload() => Utility.ExcuteCommand(Settings.DownloadCommand);
    }
}