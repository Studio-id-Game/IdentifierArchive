namespace StudioIdGames.IdentifierArchiveCore.Files
{

    public sealed class TextFileController<TData> : FileController<TData, TextFileController<TData>>
        where TData : TextFileObject<TData>, new()
    {
        public sealed override TData? FromBytes(byte[]? bytes)
        {
            if(bytes == null) return null;
            return new TData() { Text = TextFileObject<TData>.Encoding.GetString(bytes) };
        }

        public sealed override byte[]? ToBytes(TData? data)
        {
            if (data == null) return null;

            return TextFileObject<TData>.Encoding.GetBytes(data.Text);
        }
    }
}