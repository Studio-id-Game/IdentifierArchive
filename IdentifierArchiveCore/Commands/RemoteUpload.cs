namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class RemoteUpload : ICommandAction
    {
        public const string CommandID = "ul";

        public const string Name = "Upload-To-Remote";

        string ICommandAction.CommandID => CommandID;

        string ICommandAction.Name => Name;

        public int Excute(CommandArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
