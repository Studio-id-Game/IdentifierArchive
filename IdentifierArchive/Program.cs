using IdentifierArchiveCore;

namespace IdentifierArchive
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("IdentifierArchive is Installed.");
                return;
            }

            if(args.Length >= 3)
            {
                var command = args[0];
                var settingsFilePath = args[1];
                var targetPath = args[2];
                var identifier = args.Length >= 4 ? args[3] : null;

                foreach (var commandDef in Enum.GetValues<Commands>())
                {
                    if (command == commandDef.GetName())
                    {
                        Console.WriteLine($"Excute command : {commandDef.GetName()}");
                        commandDef.Excute(settingsFilePath: settingsFilePath, targetPath: targetPath, identifier: identifier);
                        return;
                    }
                }

                Console.WriteLine("ERROR : Unknown Command");
                return;
            }

            Console.WriteLine("ERROR : Unknown Command");
        }
    }
}