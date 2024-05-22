using Utf8Json;

namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public sealed class JsonFileController<TData> : FileController<TData, JsonFileController<TData>> 
        where TData : JsonFileObject<TData>
    {
        public override TData? FromBytes(byte[]? bytes)
        {
            try
            {
                return JsonSerializer.Deserialize<TData>(bytes);
            }
            catch (Exception e)
            {
                ConsoleUtility.WriteError(e);
                return default;
            }
        }

        public override byte[]? ToBytes(TData? data)
        {
            try
            {
                return JsonSerializer.PrettyPrintByteArray(JsonSerializer.Serialize(data));
            }
            catch (Exception e)
            {
                ConsoleUtility.WriteError(e);
                return null;
            }
        }
    }
}