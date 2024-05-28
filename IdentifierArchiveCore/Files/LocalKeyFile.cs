using System.IO;
using System.Runtime.Serialization;
using Utf8Json;

namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public class LocalKeyFile : JsonFileObject<LocalKeyFile>
    {
        public const string ScreenNameStatic = "LocalKey";
        public const string FileNameStatic = ".localkey~";

        public Dictionary<string, string> List { get; set; } = new() { ["%MyPassWord%"] = "PassWord12345" };

        [IgnoreDataMember]
        public override string ScreenName => ScreenNameStatic;
        
        [IgnoreDataMember]
        public override string FileName => FileNameStatic;

        public override void CopyFrom(LocalKeyFile other)
        {
            List = new(other.List);
        }
    }
}