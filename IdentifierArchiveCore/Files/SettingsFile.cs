using Utf8Json;

namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public static class SettingsFile
    {
        public const string SETTINGS_PATH = "%SETTINGS_PATH%";
        public const string TARGET_PATH = "%TARGET_PATH%";
        public const string TARGET_NAME = "%TARGET_NAME%";
        public const string IDENTIFIER = "%IDENTIFIER%";
        public const string WORK_SPACE = "%WORK_SPACE%";
        public const string ZIP_PATH = "%ZIP_PATH%";

        public const string FileName = "identifierArchiveSettings.json";

        public static byte[] DefaultValue { get; } = new Data().ToBytes();

        public static Data FromBytes(byte[] bytes)
        {
            return JsonSerializer.Deserialize<Data>(bytes);
        }

        public class Data
        {
            public string TargetBasePath { get; set; } = "./";
            public string ZipPath { get; set; } = $"{TARGET_PATH}.ZipArchives~/{IDENTIFIER}.7z";
            public string ZipCommand { get; set; } = $"C:\\Program Files\\7-Zip\\7z.exe a -uq0 -p%MyPassWord% -xr!*.identifier {ZIP_PATH} {TARGET_PATH}";
            public string UnzipCommand { get; set; } = $"C:\\Program Files\\7-Zip\\7z.exe x -p%MyPassWord% {ZIP_PATH} -o{TARGET_PATH}";
            public string UploadCommand { get; set; } = $"\"GoogleDriveStrage.exe\" u {SETTINGS_PATH}.localkeygdrive~ %GoogleDriveStrage.RootFolderID% {TARGET_NAME} {ZIP_PATH}";
            public string DownloadCommand { get; set; } = $"\"GoogleDriveStrage.exe\" d {SETTINGS_PATH}.localkeygdrive~ %GoogleDriveStrage.RootFolderID% {TARGET_NAME} {ZIP_PATH}";

            public byte[] ToBytes()
            {
                return JsonSerializer.PrettyPrintByteArray(JsonSerializer.Serialize(this));
            }

            public void SetEnvironment(string? targetName = null, string? identifier = null, string? settingsPath = null, LocalKeyFile.Data? localKey = null)
            {
                if (localKey != null)
                {
                    foreach (var key in localKey.KeySet)
                    {
                        Replace(key.Key, key.Value);
                    }
                }
                Replace(SETTINGS_PATH, settingsPath);
                Replace(TARGET_PATH, new DirectoryInfo($"{TargetBasePath.TrimEnd('/', '\\')}/{targetName}").FullName);
                Replace(TARGET_NAME, targetName);
                Replace(IDENTIFIER, identifier);
                Replace(ZIP_PATH, new DirectoryInfo(ZipPath).FullName);
            }

            private void Replace(string var, string? value)
            {
                if (value != null)
                {
                    ZipPath = ZipPath.Replace(var, value);
                    ZipCommand = ZipCommand.Replace(var, value);
                    UnzipCommand = UnzipCommand.Replace(var, value);
                    UploadCommand = UploadCommand.Replace(var, value);
                    DownloadCommand = DownloadCommand.Replace(var, value);
                }
            }
        }
    }
}