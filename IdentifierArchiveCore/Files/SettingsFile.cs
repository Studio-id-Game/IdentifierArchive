using Utf8Json;

namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public static class SettingsFile
    {
        public const string FileName = "identifierArchiveSettings.json";

        public static byte[] DefaultValue { get; } = new Data().ToBytes();

        public static Data FromBytes(byte[] bytes)
        {
            return JsonSerializer.Deserialize<Data>(bytes);
        }

        public struct Data
        {

            public Data()
            {
            }

            public string WorkSpace { get; set; } = ".identifierArchive/";
            public string ZipCommand { get; set; } = "C:/Program Files/7-Zip/7z.exe a -pPassWord %OUTPUT_PATH%.7z %INPUT_PATH%";
            public string UnzipCommand { get; set; } = "C:/Program Files/7-Zip/7z.exe x -pPassWord %OUTPUT_PATH%.7z";
            public string UploadCommand { get; set; } = "";
            public string DownloadCommand { get; set; } = "";

            readonly public byte[] ToBytes()
            {
                return JsonSerializer.PrettyPrintByteArray(JsonSerializer.Serialize(this));
            }
        }

    }
}