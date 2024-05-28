using StudioIdGames.IdentifierArchiveCore.Commands;

namespace StudioIdGames.IdentifierArchiveCore
{
    public static class Excuter
    {
        public static int Excute(ReadOnlySpan<string> args)
        {
            return CommandActions.ExcuteAll(args);
        }
    }
}