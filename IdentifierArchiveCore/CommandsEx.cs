namespace IdentifierArchiveCore
{
    public static class CommandsEx
    {
        public static string GetName(this Commands command)
        {
            return command switch
            {
                Commands.Push => "push",
                Commands.Pull => "pull",
                _ => "none",
            };
        }

        public static void Excute(this Commands command, string settingsFilePath, string targetPath, string? identifier)
        {
            switch(command)
            {
                case Commands.Push:
                    
                    Actions.Push(new() 
                    { 
                        SettingsFilePath = settingsFilePath, 
                        TargetPath = targetPath, 
                        Identifier = identifier,
                    });

                    break;

                case Commands.Pull:
                    
                    Actions.Pull(new()
                    {
                        SettingsFilePath = settingsFilePath,
                        TargetPath = targetPath,
                        Identifier = identifier,
                    });

                    break;
            }
        }
    }
}