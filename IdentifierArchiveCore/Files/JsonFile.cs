using Utf8Json;

namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public class JsonFile<Self> where Self : JsonFile<Self>, new()
    {
        public static Self DefaultValue { get; } = new Self();
        public static byte[]? DefaultBytes { get; } = new Self().ToBytes();
        
        public static bool CreateDefaultFile(FileInfo? fileInfo)
        {
            return new Self().ToFile(fileInfo);
        }

        public static Self? FromBytes(byte[] bytes)
        {
            try
            {
                return JsonSerializer.Deserialize<Self>(bytes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static Self? FromFile(FileInfo? fileInfo)
        {
            try
            {
                if (fileInfo?.Exists ?? false)
                {
                    var bytes = File.ReadAllBytes(fileInfo.FullName);
                    return FromBytes(bytes);
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public byte[]? ToBytes()
        {
            try
            {
                return JsonSerializer.PrettyPrintByteArray(JsonSerializer.Serialize((Self)this));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public bool ToFile(FileInfo? fileInfo)
        {
            if (fileInfo == null)
            {
                Console.WriteLine("FileInfo is null");
            }
            else
            {
                var bytes = ToBytes();
                if (bytes != null)
                {
                    try
                    {
                        File.WriteAllBytes(fileInfo.FullName, bytes);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }

            return false;
        }
    } 
}