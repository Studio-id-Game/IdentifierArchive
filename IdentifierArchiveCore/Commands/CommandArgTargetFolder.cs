namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandArgTargetFolder : CommandArg
    {
        public CommandArgTargetFolder() { }

        public CommandArgTargetFolder(CommandArgTargetFolder sauce) : base(sauce) { }

        public override string Name => $"--target-folder";

        public override string ArgID => "-tf";

        public override CommandArg Copy()
        {
            return new CommandArgTargetFolder(this);
        }

        public override string ToString()
        {
            return ToStringAsText();
        }
    }
}
