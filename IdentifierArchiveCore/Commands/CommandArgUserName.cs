namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandArgUserName : CommandArg
    {
        public CommandArgUserName() { }

        public CommandArgUserName(CommandArgUserName sauce) : base(sauce) { }

        public override string Name => $"-{nameof(CommandArgUserName)}";

        public override string ArgID => "-username";

        public override CommandArg Copy()
        {
            return new CommandArgUserName(this);
        }

        public override string ToString()
        {
            return ToStringAsText();
        }
    }
}
