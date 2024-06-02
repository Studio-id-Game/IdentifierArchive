namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandArgAutoFolderCreate : CommandArg
    {
        public CommandArgAutoFolderCreate() { }

        public CommandArgAutoFolderCreate(CommandArgAutoFolderCreate sauce) : base(sauce) { }

        public override string Name => $"--auto-folder-create";

        public override string ArgID => "-auto-folder-c";

        public override CommandArg Copy()
        {
            return new CommandArgAutoFolderCreate(this);
        }

        public override string ToString()
        {
            return ToStringAsFlag();
        }
    }
}
