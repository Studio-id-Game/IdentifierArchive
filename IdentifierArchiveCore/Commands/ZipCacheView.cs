namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class ZipCacheView : ICommandAction
    {
        public const string CommandID = "cv";

        public const string Name = "Zip-Cache-View";

        string ICommandAction.CommandID => CommandID;

        string ICommandAction.Name => Name;

        public int Excute(CommandArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
