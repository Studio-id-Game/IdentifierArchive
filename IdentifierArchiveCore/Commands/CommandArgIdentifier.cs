namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandArgIdentifier : CommandArg
    {
        public CommandArgIdentifier() { }

        public CommandArgIdentifier(CommandArgIdentifier sauce) : base(sauce) { }

        public override string Name => $"--identifier";

        public override string ArgID => "-id";

        public override CommandArg Copy()
        {
            return new CommandArgIdentifier(this);
        }
        public override string ToString()
        {
            return ToStringAsText();
        }
    }
}
