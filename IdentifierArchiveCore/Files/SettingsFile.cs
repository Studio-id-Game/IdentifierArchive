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
        public static readonly string ZIP_FOLDER_ABS = $"%{nameof(ZIP_FOLDER_ABS)}%";
        public static readonly string ZIP_FILE_ABS = $"%{nameof(ZIP_FILE_ABS)}%";
        public static readonly string ZIP_EXTENSION = $"%{nameof(ZIP_EXTENSION)}%";
        public static readonly string UPLOAD_FILE = $"%{nameof(UPLOAD_FILE)}%";
        public static readonly string DOWNLOAD_FILE = $"%{nameof(DOWNLOAD_FILE)}%";

        public static readonly PathIdentity TARGET_FOLDER = new(nameof(TARGET_FOLDER));

        public const string ScreenNameStatic = "Settings";
        public const string FileNameStatic = "IdentifierArchiveSettings.json";

        /// <summary>
        /// 保安上隠蔽したいファイルを配置するためのフォルダを表す絶対パス
        /// </summary>
        public string LocalkeyFolderAbsolute { get; set; } = $"{SETTINGS_FOLDER_ABS}\\IdentifierArchiveWork~\\LocalKeys~\\";

        /// <summary>
        /// %TARGET_FOLDER% に対応する圧縮ファイルを保存するフォルダを表す絶対パス。ファイルは %ZIP_FOLDER_ABS%/%IDENTIFIER%.%ZIP_EXTENSION%に保存されます。
        /// </summary>
        public string ZipFolderAbsolute { get; set; } = $"%CacheRoot%\\IdentifierArchiveCache~\\%ProjectName%\\{TARGET_FOLDER.Path}\\";

        /// <summary>
        /// 圧縮ファイルの拡張子
        /// </summary>
        public string ZipExtension { get; set; } = ".7z";

        /// <summary>
        /// %TARGET_FOLDER_ABS% 内の .identifier ファイル以外を圧縮して %ZIP_FILE_ABS% として保存するコマンド 
        /// </summary>
        public string ZipCommand { get; set; } = $"%7z% a -t7z -ssw -m0=LZMA2 -mhe=on -p%7z.PassWord% {ZIP_FILE_ABS} {TARGET_FOLDER.PathAbsolute} -uq0 -xr!*.identifier";

        /// <summary>
        /// %ZIP_FILE_ABS% を解凍して %TARGET_FOLDER_ABS% に配置するコマンド 
        /// </summary>
        public string UnzipCommand { get; set; } = $"%7z% x -t7z -ssw -m0=LZMA2 -mhe=on -p%7z.PassWord% {ZIP_FILE_ABS} -o{TARGET_FOLDER.ParentAbsolute} -aoa";

        /// <summary>
        /// %UPLOAD_FILE% が表すファイルを、%TARGET_FOLDER% から一意に定まるクラウドストレージフォルダにアップロードするコマンド
        /// </summary>
        public string UploadCommand { get; set; } = $"%GoogleDriveStrage% u {LOCALKEY_FOLDER_ABS}\\%GoogleDriveStrage.KeyFileName% %GoogleDriveStrage.RootFolderID% {TARGET_FOLDER.Path} {UPLOAD_FILE}";

        /// <summary>
        /// %DOWNLOAD_FILE% が表すファイルを、%TARGET_FOLDER% から一意に定まるクラウドストレージフォルダからダウンロードするコマンド
        /// </summary>
        public string DownloadCommand { get;set; } = $"%GoogleDriveStrage% d {LOCALKEY_FOLDER_ABS}\\%GoogleDriveStrage.KeyFileName% %GoogleDriveStrage.RootFolderID% {TARGET_FOLDER.Path} {DOWNLOAD_FILE}";

        /// <summary>
        /// Gitの実行ファイルにアクセスするためのパス
        /// </summary>
        public string GitExePath { get; set; } = "git";

        [IgnoreDataMember]
        public override string ScreenName => ScreenNameStatic;

        [IgnoreDataMember]
        public override string FileName => FileNameStatic;

        [IgnoreDataMember]
        public SettingsFile? SafeView { get; private set; }

        [IgnoreDataMember]
        public DirectoryInfo? SettingsFolderInfo { get; private set; }

        [IgnoreDataMember]
        public DirectoryInfo? TargetFolderInfo { get; private set; }

        [IgnoreDataMember]
        public LocalKeyFile? LocalKeyFile { get; private set; }

        [IgnoreDataMember]
        public string? Identifier { get; private set; }

        [IgnoreDataMember]
        public bool IsZipPathReplaced { get; private set; }

        private void Replace(string var, string? value)
        {
            if (value != null)
            {
                LocalkeyFolderAbsolute = LocalkeyFolderAbsolute.Replace(var, value);
                ZipFolderAbsolute = ZipFolderAbsolute.Replace(var, value);
                ZipCommand = ZipCommand.Replace(var, value);
                UnzipCommand = UnzipCommand.Replace(var, value);
                UploadCommand = UploadCommand.Replace(var, value);
                DownloadCommand = DownloadCommand.Replace(var, value);
                GitExePath = GitExePath.Replace(var, value);
            }
        }

        private void Replace<T>(PathIdentity var, PathIdentityInfo<T> value) where T : FileSystemInfo
        {
            if (value != null)
            {
                LocalkeyFolderAbsolute = value.Replace(LocalkeyFolderAbsolute, var);
                ZipFolderAbsolute = value.Replace(ZipFolderAbsolute, var);
                ZipCommand = value.Replace(ZipCommand, var);
                UnzipCommand = value.Replace(UnzipCommand, var);
                UploadCommand = value.Replace(UploadCommand, var);
                DownloadCommand = value.Replace(DownloadCommand, var);
                GitExePath = value.Replace(GitExePath, var);
            }
        }

        private void ReplaceSettingsPath(DirectoryInfo settingsFolderInfo)
        {
            if(SafeView == null)
            {
                SafeView = new();
                SafeView.CopyFrom(this);
            }

            if(SettingsFolderInfo == null)
            {
                var settingsFileInfo = new FileInfo($"{settingsFolderInfo}/{FileName}");

                SettingsFolderInfo = settingsFolderInfo;
                Replace(SETTINGS_FOLDER_ABS, settingsFolderInfo.FullName);
                Replace(SETTINGS_FILE_ABS, settingsFileInfo.FullName);

                SafeView.SettingsFolderInfo = settingsFolderInfo;
                SafeView.Replace(SETTINGS_FOLDER_ABS, settingsFolderInfo.FullName);
                SafeView.Replace(SETTINGS_FILE_ABS, settingsFileInfo.FullName);
            }
            else
            {
                throw new InvalidOperationException("SettingsPath is already replaced.");
            }
        }

        private void ReplaceTargetPath(DirectoryInfo targetFolderInfo)
        {
            if (SafeView == null)
            {
                SafeView = new();
                SafeView.CopyFrom(this);
            }

            if (TargetFolderInfo == null)
            {
                if(SettingsFolderInfo != null)
                {
                    var targetFolderPathInfo = new PathIdentityInfo<DirectoryInfo>(targetFolderInfo, SettingsFolderInfo);
                    TargetFolderInfo = targetFolderInfo;
                    Replace(TARGET_FOLDER, targetFolderPathInfo);
                    Replace(ZIP_FOLDER_ABS, GetZipFolderInfo().FullName);
                    SafeView.TargetFolderInfo = targetFolderInfo;
                    SafeView.Replace(TARGET_FOLDER, targetFolderPathInfo);
                    SafeView.Replace(ZIP_FOLDER_ABS, SafeView.GetZipFolderInfo().FullName);
                }
                else
                {
                    throw new InvalidOperationException("SettingsPath must be replaced before TargetPath can be replaced.");
                }
            }
            else
            {
                throw new InvalidOperationException("TargetPath is already replaced.");
            }
        }

        private void ReplaceIdentifier(string identifier)
        {
            if (SafeView == null)
            {
                SafeView = new();
                SafeView.CopyFrom(this);
            }

            if (Identifier == null)
            {
                Identifier = identifier;
                Replace(IDENTIFIER, identifier);
                Replace(ZIP_FILE_ABS, GetZipFileInfo().FullName);

                SafeView.Identifier = identifier;
                SafeView.Replace(IDENTIFIER, identifier);
                SafeView.Replace(ZIP_FILE_ABS, SafeView.GetZipFileInfo().FullName);
            }
            else
            {
                throw new InvalidOperationException("Identifier is already replaced.");
            }
        }

        private void ReplaceLocalKey()
        {
            if (SafeView == null)
            {
                SafeView = new();
                SafeView.CopyFrom(this);
            }

            if (LocalKeyFile == null)
            {
                var localKeyFolder = GetLocalkeyFolderInfo();
                var localKey = new LocalKeyFile();
                localKey.FromFile(localKeyFolder);
                LocalKeyFile = localKey;

                Replace(LOCALKEY_FOLDER_ABS, localKeyFolder.FullName);
                foreach (var item in localKey.List)
                {
                    Replace(item.Key, item.Value);
                }
            }
            else
            {
                throw new InvalidOperationException("LocalKey is already replaced.");
            }
        }

        public SettingsFile GetReplaced(DirectoryInfo? settingsFolderInfo = null, DirectoryInfo? targetFolderInfo = null, string? identifier = null)
        {
            var file = new SettingsFile();
            file.CopyFrom(this);

            if (settingsFolderInfo != null)
            {
                file.ReplaceSettingsPath(settingsFolderInfo);
                file.ReplaceLocalKey();
            }

            if (targetFolderInfo != null)
            {
                file.ReplaceTargetPath(targetFolderInfo);
            } 

            if(identifier != null)
            {
                file.ReplaceIdentifier(identifier);
            }

            return file;
        }

        public override void CopyFrom(SettingsFile other)
        {
            LocalkeyFolderAbsolute = other.LocalkeyFolderAbsolute;
            ZipFolderAbsolute = other.ZipFolderAbsolute;
            ZipExtension = other.ZipExtension;
            ZipCommand = other.ZipCommand;
            UnzipCommand = other.UnzipCommand;
            UploadCommand = other.UploadCommand;
            DownloadCommand = other.DownloadCommand;
            GitExePath = other.GitExePath;

            SafeView = other.SafeView;
            SettingsFolderInfo = other.SettingsFolderInfo;
            TargetFolderInfo = other.TargetFolderInfo;
            Identifier = other.Identifier;  
            LocalKeyFile = other.LocalKeyFile;
            IsZipPathReplaced = other.IsZipPathReplaced;
        }

        public int ExcuteZip()
        {
            Console.WriteLine($"Excute Zip Command :\n{SafeView?.ZipCommand}\n");
            return ConsoleUtility.ExcuteCommand(ZipCommand);
        }

        public int ExcuteUnzip()
        {
            Console.WriteLine($"Excute Unzip Command :\n{SafeView?.UnzipCommand}\n");
            return ConsoleUtility.ExcuteCommand(UnzipCommand);
        }

        public int ExcuteUpload(FileInfo uploadFile)
        {
            Console.WriteLine($"Excute Upload Command :\n{SafeView?.UploadCommand.Replace(UPLOAD_FILE, uploadFile.FullName)}\n");
            return ConsoleUtility.ExcuteCommand(UploadCommand.Replace(UPLOAD_FILE, uploadFile.FullName));
        }

        public int ExcuteDownload(FileInfo downloadFile)
        {
            Console.WriteLine($"Excute Download Command :\n{SafeView?.DownloadCommand.Replace(DOWNLOAD_FILE, downloadFile.FullName)}\n");
            return ConsoleUtility.ExcuteCommand(DownloadCommand.Replace(DOWNLOAD_FILE, downloadFile.FullName));
        }

        public DirectoryInfo GetLocalkeyFolderInfo()
        {
            return new(LocalkeyFolderAbsolute);
        }

        public DirectoryInfo GetZipFolderInfo()
        {
            return new(ZipFolderAbsolute);
        }

        public FileInfo GetZipFileInfo(string? identifier = null)
        {
            return new($"{ZipFolderAbsolute}/{identifier ?? Identifier}.{ZipExtension.TrimStart('.')}");
        }
    }
}