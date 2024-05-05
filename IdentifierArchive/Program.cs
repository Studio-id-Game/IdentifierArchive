using StudioIdGames.IdentifierArchiveCore;

namespace StudioIdGames.IdentifierArchive
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var info = Excuter.Excute(args);

            Console.WriteLine(info.IsError ? $"ERROR : {info.Message}" : info.Message);
        }
    }
}