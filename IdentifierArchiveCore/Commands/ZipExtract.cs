namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class ZipExtract : ICommandAction
    {
        public const string CommandID = "zx";

        public const string Name = "Zip-Extract";

        string ICommandAction.CommandID => CommandID;

        string ICommandAction.Name => Name;

        public int Excute(CommandArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
