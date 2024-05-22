namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public abstract class JsonFileObject<TSelf> : FileObject<JsonFileController<TSelf>, TSelf>
        where TSelf : JsonFileObject<TSelf>
    {
    }
}