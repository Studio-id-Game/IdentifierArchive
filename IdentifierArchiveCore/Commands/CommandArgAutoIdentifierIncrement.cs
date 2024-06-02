namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandArgAutoIdentifierIncrement : CommandArg
    {
        public CommandArgAutoIdentifierIncrement() { }

        public CommandArgAutoIdentifierIncrement(CommandArgAutoIdentifierIncrement sauce) : base(sauce) { }

        public override string Name => $"--auto-identifier-increment";

        public override string ArgID => "-auto-id-incr";

        public override CommandArg Copy()
        {
            return new CommandArgAutoIdentifierIncrement(this);
        }

        public override string ToString()
        {
            return ToStringAsFlag();
        }
    }
}
