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
        public static readonly string ZIP_FILE_ABS = $"%{nameof(ZIP_FILE_ABS)}%";
        public static readonly string ZIP_FOLDER_ABS = $"%{nameof(ZIP_FOLDER_ABS)}%";
        public static readonly PathIdentity TARGET_FOLDER = new(nameof(TARGET_FOLDER));

        public const string FileName = "IdentifierArchiveSettings.json";

        /// <summary>
        /// 保安上隠蔽したいファイルを配置するためのフォルダを表す絶対パス
        /// </summary>
        public string LocalkeyFolderAbsolute { get; set; } = $"D:/LocalKeys~/ProjectName";

        /// <summary>
        /// %TARGET_FOLDER% と 識別子 %IDENTIFIER% に対応する圧縮ファイルを表す、%SETTINGS_FILE_PARENT% からの相対ファイルパス
        /// </summary>
        public string ZipFile { get; set; } = $"{TARGET_FOLDER.Path}.ZipArchives~/{IDENTIFIER}.7z";

        /// <summary>
        /// %TARGET_FOLDER_ABS% 内の .identifier ファイル以外を圧縮して %ZIP_FILE_ABS% として保存するコマンド 
        /// </summary>
        public string ZipCommand { get;set; } = $"C:\\Program Files\\7-Zip\\7z.exe a -uq0 -p%MyPassWord% -xr!*.identifier {ZIP_FILE_ABS} {TARGET_FOLDER.PathAbsolute}";

        /// <summary>
        /// %ZIP_FILE_ABS% を解凍して %TARGET_FOLDER_ABS% に配置するコマンド 
        /// </summary>
        public string UnzipCommand { get; set; } = $"C:\\Program Files\\7-Zip\\7z.exe x -p%MyPassWord% {ZIP_FILE_ABS} -o{TARGET_FOLDER.PathAbsolute}";

        /// <summary>
        /// %ZIP_FILE_ABS% を、%ZIP_FILE_PARENT% から一意に定まるクラウドストレージにアップロードするコマンド
        /// </summary>
        public string UploadCommand { get; set; } = $"\"GoogleDriveStrage.exe\" u {LOCALKEY_FOLDER_ABS}/.gdrivelocalkey~ %GoogleDriveStrage.RootFolderID% {ZIP_FOLDER_ABS} {ZIP_FILE_ABS}";

        /// <summary>
        /// %ZIP_FILE_ABS% を、%ZIP_FILE_PARENT% から一意に定まるクラウドストレージからダウンロードするコマンド
        /// </summary>
        public string DownloadCommand { get;set; } = $"\"GoogleDriveStrage.exe\" d {LOCALKEY_FOLDER_ABS}/.gdrivelocalkey~ %GoogleDriveStrage.RootFolderID% {ZIP_FOLDER_ABS} {ZIP_FILE_ABS}";

        public void Replace(string var, string? value)
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

        public void Replace<T>(PathIdentity var, PathIdentityInfo<T> value) where T : FileSystemInfo
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
    }
}