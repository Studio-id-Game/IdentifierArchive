using System.Globalization;
using System.IO.Compression;
using System.Reflection;
using Utf8Json;

namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public class SettingsFile : JsonFile<SettingsFile>
    {
        public const string IDENTIFIER = $"%{nameof(IDENTIFIER)}%";
        public static readonly string SETTINGS_FILE_ABS = $"%{nameof(SETTINGS_FILE_ABS)}%";
        public static readonly string SETTINGS_FOLDER_ABS = $"%{nameof(SETTINGS_FOLDER_ABS)}%";
        public static readonly string LOCALKEY_FOLDER_ABS = $"%{nameof(LOCALKEY_FOLDER_ABS)}%";
        public static readonly PathIdentity ZIP_FILE = new(nameof(ZIP_FILE));
        public static readonly PathIdentity TARGET_FOLDER = new(nameof(TARGET_FOLDER));

        public const string FileName = "IdentifierArchiveSettings.json";

        /// <summary>
        /// 保安上隠蔽したいファイルを配置するためのフォルダを表す絶対パス
        /// </summary>
        public string LocalkeyFolderAbsolute { get; private set; } = $"D:/LocalKeys~/ProjectName/{TARGET_FOLDER.Path}";

        /// <summary>
        /// %TARGET_FOLDER% と 識別子 %IDENTIFIER% に対応する圧縮ファイルを表す、%SETTINGS_FILE_PARENT% からの相対ファイルパス
        /// </summary>
        public string ZipFile { get; private set; } = $"{TARGET_FOLDER.Path}.ZipArchives~/{IDENTIFIER}.7z";

        /// <summary>
        /// %TARGET_FOLDER_ABS% 内の .identifier ファイル以外を圧縮して %ZIP_FILE_ABS% として保存するコマンド 
        /// </summary>
        public string ZipCommand { get; private set; } = $"C:\\Program Files\\7-Zip\\7z.exe a -uq0 -p%MyPassWord% -xr!*.identifier {ZIP_FILE.PathAbsolute} {TARGET_FOLDER.PathAbsolute}";

        /// <summary>
        /// %ZIP_FILE_ABS% を解凍して %TARGET_FOLDER_ABS% に配置するコマンド 
        /// </summary>
        public string UnzipCommand { get; private set; } = $"C:\\Program Files\\7-Zip\\7z.exe x -p%MyPassWord% {ZIP_FILE.PathAbsolute} -o{TARGET_FOLDER.PathAbsolute}";

        /// <summary>
        /// %ZIP_FILE_ABS% を、%ZIP_FILE_PARENT% から一意に定まるクラウドストレージにアップロードするコマンド
        /// </summary>
        public string UploadCommand { get; private set; } = $"\"GoogleDriveStrage.exe\" u {LOCALKEY_FOLDER_ABS}/.gdrivelocalkey~ %GoogleDriveStrage.RootFolderID% {ZIP_FILE.Parent} {ZIP_FILE.PathAbsolute}";

        /// <summary>
        /// %ZIP_FILE_ABS% を、%ZIP_FILE_PARENT% から一意に定まるクラウドストレージからダウンロードするコマンド
        /// </summary>
        public string DownloadCommand { get; private set; } = $"\"GoogleDriveStrage.exe\" d {LOCALKEY_FOLDER_ABS}/.gdrivelocalkey~ %GoogleDriveStrage.RootFolderID% {ZIP_FILE.Parent} {ZIP_FILE.PathAbsolute}";

        private void Replace(string var, string? value)
        {
            if (value != null)
            {
                LocalkeyFolderAbsolute = LocalkeyFolderAbsolute.Replace(var, value);
                ZipFile = ZipFile.Replace(var, value);
                ZipCommand = ZipCommand.Replace(var, value);
                UnzipCommand = UnzipCommand.Replace(var, value);
                UploadCommand = UploadCommand.Replace(var, value);
                DownloadCommand = DownloadCommand.Replace(var, value);
            }
        }

        private void Replace<T>(PathIdentity var, PathIdentityInfo<T> value) where T : FileSystemInfo
        {
            if (value != null)
            {
                LocalkeyFolderAbsolute = value.Replace(LocalkeyFolderAbsolute, var);
                ZipFile = value.Replace(ZipFile, var);
                ZipCommand = value.Replace(ZipCommand, var);
                UnzipCommand = value.Replace(UnzipCommand, var);
                UploadCommand = value.Replace(UploadCommand, var);
                DownloadCommand = value.Replace(DownloadCommand, var);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settingsFolderPath">実行ファイルから見た SettingsFile のあるフォルダーの場所</param>
        /// <param name="targetFolderPath">SettingsFile から見た ターゲットフォルダーの場所</param>
        public static Controller GetController(string settingsFolderPath, string targetFolderPath)
        {
            return new Controller(settingsFolderPath, targetFolderPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settingsFolderPath">実行ファイルから見た SettingsFile のあるフォルダーの場所</param>
        /// <param name="targetFolderPath">SettingsFile から見た ターゲットフォルダーの場所</param>
        public class Controller(string settingsFolderPath, string targetFolderPath)
        {
            /// <summary>
            /// 実行ファイルから見た SettingsFile の場所
            /// </summary>
            public string SettingsFolderPath { get; } = settingsFolderPath;

            /// <summary>
            /// SettingsFile から見た ターゲットフォルダーの場所
            /// </summary>
            public string? TargetFolderPath { get; } = targetFolderPath;

            public SettingsFile Settings { get; private set; } = new();

            public DirectoryInfo SettingsFolderInfo => new(SettingsFolderPath);

            public FileInfo SettingsFileInfo => new($"{SettingsFolderPath}/{FileName}");

            public DirectoryInfo LocalkeyFolderInfo => new(Settings.LocalkeyFolderAbsolute);

            public FileInfo LocalkeyFileInfo => new($"{Settings.LocalkeyFolderAbsolute}/{LocalKeyFile.FileName}");

            public PathIdentityInfo<FileInfo> ZipFileInfo => new(new(Settings.ZipFile), SettingsFolderInfo);

            public PathIdentityInfo<DirectoryInfo> TargetFolderInfo => new(new($"{SettingsFolderPath}/{TargetFolderPath}"), SettingsFolderInfo);

            public FileInfo IdentifierFileInfo => new($"{SettingsFolderPath}/{TargetFolderPath}/{IdentifierFile.ArchiveFileName}");

            public FileInfo CurrentIdentifierFileInfo => new($"{SettingsFolderPath}/{TargetFolderPath}/{IdentifierFile.CurrentFileName}");

            /// <summary>
            /// 設定ファイルを読み込み、可能であれば環境変数を適用します。
            /// </summary>
            /// <param name="targetFolderPath">SettingsFile から見た ターゲットフォルダーの場所</param>
            /// <param name="identifier">ターゲットとする識別子</param>
            /// <returns></returns>
            public bool TryLoad(string identifier)
            {
                bool res = false;
                var settings = FromFile(SettingsFileInfo);
                if (settings != null)
                {
                    Settings = settings;
                    res = true;
                }

                Settings.Replace(IDENTIFIER, identifier);
                Settings.Replace(TARGET_FOLDER, TargetFolderInfo);
                Settings.Replace(SETTINGS_FOLDER_ABS, SettingsFolderInfo.FullName);
                Settings.Replace(SETTINGS_FILE_ABS, SettingsFileInfo.FullName);
                Settings.Replace(LOCALKEY_FOLDER_ABS, LocalkeyFolderInfo.FullName);
                Settings.Replace(ZIP_FILE, ZipFileInfo);

                var localkeyFile = LocalKeyFile.FromFile(LocalkeyFileInfo);
                if (localkeyFile != null)
                {
                    foreach (var item in localkeyFile.KeySet)
                    {
                        Settings.Replace(item.Key, item.Value);
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
}