namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandArgAutoInteractive : CommandArg
    {
        public CommandArgAutoInteractive() { }

        public CommandArgAutoInteractive(CommandArgAutoInteractive sauce) : base(sauce) { }

        public override string Name => $"--auto-interactive";

        public override string ArgID => "-auto-interactive";

        public override CommandArg Copy()
        {
            return new CommandArgAutoInteractive(this);
        }

        public override string ToString()
        {
            return ToStringAsFlag();
        }
    }
}
