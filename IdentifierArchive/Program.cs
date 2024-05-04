using IdentifierArchiveCore;

namespace IdentifierArchive
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var info = IdentifierArchiveCore.IdentifierArchiveCore.Excute(args);

            Console.WriteLine(info.IsError ? $"ERROR : {info.Message}" : info.Message);
        }
    }
}