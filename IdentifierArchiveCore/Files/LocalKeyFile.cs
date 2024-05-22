using System.IO;
using Utf8Json;

namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public class LocalKeyFile : JsonFileObject<LocalKeyFile>
    {
        public const string ScreenNameStatic = "LocalKey";
        public const string FileNameStatic = ".localkey~";

        public Dictionary<string, string> List { get; set; } = new() { ["%MyPassWord%"] = "PassWord12345" };

        public override string ScreenName => ScreenNameStatic;

        public override string FileName => FileNameStatic;

        public override void CopyFrom(LocalKeyFile other)
        {
            List = new(other.List);
        }
    }
}