using System.IO;
using Utf8Json;

namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public class LocalKeyFile : JsonFile<LocalKeyFile>
    {
        public const string FileName = "IdentifierArchiveSettings.localkey~";

        public Dictionary<string, string> KeySet { get; set; } = new() { ["%MyPassWord%"] = "PassWord12345" };
    }
}