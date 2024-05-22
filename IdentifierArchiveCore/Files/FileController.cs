using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public abstract class FileController<TData>
    {
        public abstract TData? FromBytes(byte[] bytes);

        public abstract byte[]? ToBytes(TData data);

        public TData? FromFile(FileInfo info, string screenName)
        {
            Console.WriteLine($"Read {screenName} file. ({info?.FullName})");

            if (info == null)
            {
                Console.WriteLine($"FileInfo is null.");
            }
            else if (ConsoleUtility.CheckFile(info, screenName))
            {
                try
                {
                    var bytes = File.ReadAllBytes(info.FullName);
                    return FromBytes(bytes);
                }
                catch (Exception e)
                {
                    ConsoleUtility.WriteError(e);
                }
            }

            Console.WriteLine($"Failed to read {screenName} file. ({info?.FullName})");
            return default;
        }

        public bool ToFile(TData data, FileInfo info, string screenName, out bool created, out bool overwrited, bool? autoCreate = false, bool? autoOverwrite = false)
        {
            Console.WriteLine($"Write {screenName} file. ({info?.FullName})");

            bool exists = false;
            created = false;
            overwrited = false;

            if (info == null)
            {
                Console.WriteLine("FileInfo is null");
            }
            else
            {
                var bytes = ToBytes(data);
                if (bytes != null)
                {
                    exists = ConsoleUtility.CheckFile(info, screenName, out created, out overwrited, autoCreate, autoOverwrite, (info) => File.WriteAllBytes(info.FullName, bytes));
                }
            }

            if (!created || !overwrited)
            {
                Console.WriteLine($"Failed to write {screenName} file. ({info?.FullName})");
            }

            return exists;
        }
    }

    public abstract class FileController<TData, TSelf> : FileController<TData> 
        where TSelf : FileController<TData, TSelf>, new()
    { 
        public static TSelf Instance { get; } = new TSelf();
    }
}