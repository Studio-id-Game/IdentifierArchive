using System.Globalization;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Serialization;
using Utf8Json;

namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public class SettingsFile : JsonFileObject<SettingsFile>
    {
        public const string IDENTIFIER = $"%{nameof(IDENTIFIER)}%";
        public static readonly string SETTINGS_FILE_ABS = $"%{nameof(SETTINGS_FILE_ABS)}%";
        public static readonly string SETTINGS_FOLDER_ABS = $"%{nameof(SETTINGS_FOLDER_ABS)}%";
        public static readonly string LOCALKEY_FOLDER_ABS = $"%{nameof(LOCALKEY_FOLDER_ABS)}%";
        public static readonly string ZIP_FILE_ABS = $"%{nameof(ZIP_FILE_ABS)}%";
        public static readonly string ZIP_FOLDER_ABS = $"%{nameof(ZIP_FOLDER_ABS)}%";
        public static readonly PathIdentity TARGET_FOLDER = new(nameof(TARGET_FOLDER));

        public const string ScreenNameStatic = "Settings";
        public const string FileNameStatic = "IdentifierArchiveSettings.json";

        /// <summary>
        /// 保安上隠蔽したいファイルを配置するためのフォルダを表す絶対パス
        /// </summary>
        public string LocalkeyFolderAbsolute { get; set; } = $"{SETTINGS_FOLDER_ABS}\\IdentifierArchiveWork~\\LocalKeys~";

        /// <summary>
        /// %TARGET_FOLDER% と 識別子 %IDENTIFIER% に対応する圧縮ファイルを表す、%SETTINGS_FILE_PARENT% からの相対ファイルパス
        /// </summary>
        public string ZipFile { get; set; } = $"%CacheRoot%\\IdentifierArchiveCache~\\%ProjectName%\\{TARGET_FOLDER.Path}\\{IDENTIFIER}.7z";

        /// <summary>
        /// %TARGET_FOLDER_ABS% 内の .identifier ファイル以外を圧縮して %ZIP_FILE_ABS% として保存するコマンド 
        /// </summary>
        public string ZipCommand { get;set; } = $"%7z% a -t7z -ssw -m0=LZMA2 -mhe=on -p%7z.PassWord% {ZIP_FILE_ABS} {TARGET_FOLDER.PathAbsolute} -uq0 -xr!*.identifier";

        /// <summary>
        /// %ZIP_FILE_ABS% を解凍して %TARGET_FOLDER_ABS% に配置するコマンド 
        /// </summary>
        public string UnzipCommand { get; set; } = $"%7z% x -t7z -ssw -m0=LZMA2 -mhe=on -p%7z.PassWord% {ZIP_FILE_ABS} -o{TARGET_FOLDER.ParentAbsolute} -aoa";

        /// <summary>
        /// %ZIP_FILE_ABS% を、%ZIP_FILE_PARENT% から一意に定まるクラウドストレージにアップロードするコマンド
        /// </summary>
        public string UploadCommand { get; set; } = $"%GoogleDriveStrage% u {LOCALKEY_FOLDER_ABS}\\%GoogleDriveStrage.KeyFileName% %GoogleDriveStrage.RootFolderID% {TARGET_FOLDER.Path} {ZIP_FILE_ABS}";

        /// <summary>
        /// %ZIP_FILE_ABS% を、%ZIP_FILE_PARENT% から一意に定まるクラウドストレージからダウンロードするコマンド
        /// </summary>
        public string DownloadCommand { get;set; } = $"%GoogleDriveStrage% d {LOCALKEY_FOLDER_ABS}\\%GoogleDriveStrage.KeyFileName% %GoogleDriveStrage.RootFolderID% {TARGET_FOLDER.Path} {ZIP_FILE_ABS}";

        [IgnoreDataMember]
        public override string ScreenName => ScreenNameStatic;

        [IgnoreDataMember]
        public override string FileName => FileNameStatic;

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

        private void ReplaceSettingsFilePath(DirectoryInfo settingsFolderInfo)
        {
            var settingsFileInfo = new FileInfo($"{settingsFolderInfo}/{FileName}");

            Replace(SETTINGS_FOLDER_ABS, settingsFolderInfo.FullName);
            Replace(SETTINGS_FILE_ABS, settingsFileInfo.FullName);
        }

        public void Replace(out SettingsFile safeView, DirectoryInfo? settingsFolderInfo = null, DirectoryInfo? targetFolderInfo = null, string? identifier = null, bool loadLocalkey = true)
        {
            if(settingsFolderInfo != null)
            {
                ReplaceSettingsFilePath(settingsFolderInfo);
                if(targetFolderInfo != null)
                {
                    var targetFolderPathInfo = new PathIdentityInfo<DirectoryInfo>(targetFolderInfo, settingsFolderInfo);
                    Replace(TARGET_FOLDER, targetFolderPathInfo);
                }
            }

            Replace(IDENTIFIER, identifier);

            safeView = new();
            safeView.CopyFrom(this);

            var localkeyFolderInfo = new DirectoryInfo(LocalkeyFolderAbsolute);
            Replace(LOCALKEY_FOLDER_ABS, localkeyFolderInfo.FullName);

            if (loadLocalkey)
            {
                var localkeyFile = new LocalKeyFile().FromFile(localkeyFolderInfo);
                if (localkeyFile != null)
                {
                    foreach (var item in localkeyFile.List)
                    {
                        Replace(item.Key, item.Value);
                    }
                }
            }

            var zipFileInfo = new FileInfo(ZipFile);
            var zipFolderInfo = zipFileInfo.Directory!;

            Replace(ZIP_FILE_ABS, zipFileInfo.FullName);
            Replace(ZIP_FOLDER_ABS, zipFolderInfo.FullName);
            safeView.Replace(ZIP_FILE_ABS, zipFileInfo.FullName);
            safeView.Replace(ZIP_FOLDER_ABS, zipFolderInfo.FullName);
        }

        public override void CopyFrom(SettingsFile other)
        {
            LocalkeyFolderAbsolute = other.LocalkeyFolderAbsolute;
            ZipFile = other.ZipFile;
            ZipCommand = other.ZipCommand;
            UnzipCommand = other.UnzipCommand;
            UploadCommand = other.UploadCommand;
            DownloadCommand = other.DownloadCommand;
        }

        public int ExcuteZip() => ConsoleUtility.ExcuteCommand(ZipCommand);

        public int ExcuteUnzip() => ConsoleUtility.ExcuteCommand(UnzipCommand);

        public int ExcuteUpload() => ConsoleUtility.ExcuteCommand(UploadCommand);

        public int ExcuteDownload() => ConsoleUtility.ExcuteCommand(DownloadCommand);

        public DirectoryInfo GetLocalkeyFolderInfo() => new(LocalkeyFolderAbsolute);

        public FileInfo GetZipFileInfo() => new(ZipFile);
    }
}