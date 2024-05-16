namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class RemoteDownload : ICommandAction
    {
        public const string CommandID = "dl";

        public const string Name = "Download-From-Remote";

        string ICommandAction.CommandID => CommandID;

        string ICommandAction.Name => Name;

        public int Excute(CommandArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
