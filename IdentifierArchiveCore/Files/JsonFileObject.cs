using Utf8Json;

namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public abstract class JsonFileObject<TSelf> : FileObject<JsonFileController<TSelf>, TSelf>
        where TSelf : JsonFileObject<TSelf>
    {
        public override string AsText()
        {
            return JsonSerializer.PrettyPrint(JsonSerializer.Serialize(Self));
        }
    }
}