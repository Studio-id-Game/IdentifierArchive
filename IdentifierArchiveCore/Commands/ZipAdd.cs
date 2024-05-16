namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class ZipAdd : ICommandAction
    {
        public const string CommandID = "za";

        public const string Name = "Zip-Add";

        string ICommandAction.CommandID => CommandID;

        string ICommandAction.Name => Name;

        public int Excute(CommandArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
