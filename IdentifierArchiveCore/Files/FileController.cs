using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public abstract class FileController<TData>
    {
        public abstract TData? FromBytes(byte[] bytes);

        public abstract byte[]? ToBytes(TData data);

        public TData? FromFile(FileInfo info, string screenName)
        {
            Console.WriteLine($"Reading {screenName} file... ({info?.FullName})");

            if (info == null)
            {
                Console.WriteLine($"FileInfo is null.\n");
            }
            else if (ConsoleUtility.CheckFile(info, screenName))
            {
                try
                {
                    var bytes = File.ReadAllBytes(info.FullName);
                    var data = FromBytes(bytes);
                    if (data == null)
                    {
                        Console.WriteLine($"Failed to read {screenName} file. ({info?.FullName})\n");
                    }
                    else
                    {
                        Console.WriteLine($"Success to read {screenName} file. ({info?.FullName})\n");
                    }
                    return data;
                }
                catch (Exception e)
                {
                    ConsoleUtility.WriteError(e);
                }
            }

            Console.WriteLine($"Failed to read {screenName} file. ({info?.FullName})\n");
            return default;
        }

        public bool ToFile(TData data, FileInfo info, string screenName, out bool created, out bool overwrited, bool? autoCreate = false, bool? autoOverwrite = false)
        {
            Console.WriteLine($"Writing {screenName} file... ({info?.FullName})\n");

            bool exists = false;
            created = false;
            overwrited = false;

            if (info == null)
            {
                Console.WriteLine("FileInfo is null\n");
            }
            else
            {
                var bytes = ToBytes(data);
                if (bytes != null)
                {
                    exists = ConsoleUtility.CheckFile(info, screenName, out created, out overwrited, autoCreate, autoOverwrite, (info) => File.WriteAllBytes(info.FullName, bytes));
                }
            }

            if (created)
            {
                Console.WriteLine($"Success to create {screenName} file. ({info?.FullName})\n");
            }
            else if (overwrited)
            {
                Console.WriteLine($"Success to overwrite {screenName} file. ({info?.FullName})\n");
            }
            else
            {
                Console.WriteLine($"Failed to write {screenName} file. ({info?.FullName})\n");
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