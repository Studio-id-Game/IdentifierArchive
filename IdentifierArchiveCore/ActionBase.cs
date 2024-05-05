namespace StudioIdGames.IdentifierArchiveCore
{
    public abstract class ActionBase
    {
        public abstract string Command { get; }

        public abstract ActionInfo? Excute(ReadOnlySpan<string> args);

    }
}