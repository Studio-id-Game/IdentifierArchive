using Utf8Json;

namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public static class LocalKeyFile
    {
        public static Data FromBytes(byte[] bytes)
        {
            return JsonSerializer.Deserialize<Data>(bytes);
        }
        public static byte[] DefaultValue { get; } = new Data().ToBytes();

        public const string FileName = ".localkey~";

        public class Data
        {
            public Dictionary<string, string> KeySet { get; set; } = new() {["%MyPassWord%"] = "PassWord12345" };

            public byte[] ToBytes()
            {
                return JsonSerializer.PrettyPrintByteArray(JsonSerializer.Serialize(this));
            }
        }

    }
}