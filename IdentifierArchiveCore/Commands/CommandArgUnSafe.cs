namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandArgUnSafe : CommandArg
    {
        public CommandArgUnSafe() { }

        public CommandArgUnSafe(CommandArgUnSafe sauce) : base(sauce) { }

        public override string? ValueText => null;

        public override bool? ValueFlag => null;

        public override string Name => "-UN-SAFE";

        public override string ArgID => "-UN-SAFE";

        public override CommandArg Copy()
        {
            return new CommandArgUnSafe(this);
        }

        public override string ToString()
        {
            return ToStringAsReaded();
        }
    }
}
