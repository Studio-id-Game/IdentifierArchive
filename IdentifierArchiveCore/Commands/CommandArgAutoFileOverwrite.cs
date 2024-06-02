namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandArgAutoFileOverwrite : CommandArg
    {
        public CommandArgAutoFileOverwrite() { }

        public CommandArgAutoFileOverwrite(CommandArgAutoFileOverwrite sauce) : base(sauce) { }

        public override string Name => $"--auto-file-overwrite";

        public override string ArgID => "-auto-file-ow";

        public override CommandArg Copy()
        {
            return new CommandArgAutoFileOverwrite(this);
        }

        public override string ToString()
        {
            return ToStringAsFlag();
        }
    }
}
