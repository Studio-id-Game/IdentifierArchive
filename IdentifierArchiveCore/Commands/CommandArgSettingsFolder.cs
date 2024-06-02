namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandArgSettingsFolder : CommandArg
    {
        public CommandArgSettingsFolder() { }

        public CommandArgSettingsFolder(CommandArgSettingsFolder sauce) : base(sauce) { }

        public override string Name => $"--settings-folder";

        public override string ArgID => "-sf";

        public override CommandArg Copy()
        {
            return new CommandArgSettingsFolder(this);
        }
        public override string ToString()
        {
            return ToStringAsText();
        }
    }
}
