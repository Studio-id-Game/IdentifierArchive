namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class ZipCacheClear : ICommandAction
    {
        public const string CommandID = "cc";

        public const string Name = "Zip-Cache-Clear";

        string ICommandAction.CommandID => CommandID;

        string ICommandAction.Name => Name;

        public int Excute(CommandArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
