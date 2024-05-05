namespace StudioIdGames.IdentifierArchiveCore
{
    public static class Excuter
    {
        public static ActionInfo Excute(ReadOnlySpan<string> args)
        {
            if (args.Length == 0)
            {
                return new()
                {
                    Message = "IdentifierArchive is Installed."
                };
            }

            var commandStr = args[0];

            return commandStr switch
            {
                Commands.PUSH => new Push().Excute(args[1..]) ?? new ActionInfo()
                {
                    Message = "Push success."
                },
                Commands.PULL => new Pull().Excute(args[1..]) ?? new ActionInfo()
                {
                    Message = "Pull success."
                },
                Commands.SETTING => new Setting().Excute(args[1..]) ?? new ActionInfo()
                {
                    Message = "Setting success."
                },
                Commands.INIT=> new Init().Excute(args[1..]) ?? new ActionInfo()
                {
                    Message = "Init success."
                },
                _ => new ActionInfo()
                {
                    IsError = true,
                    Message = "Unknown command."
                }
            };
        }
    }
}