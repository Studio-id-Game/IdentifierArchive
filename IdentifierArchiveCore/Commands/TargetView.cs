namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class TargetView : ICommandAction
    {
        public const string CommandID = "tv";

        public const string Name = "Target-View";

        string ICommandAction.CommandID => CommandID;

        string ICommandAction.Name => Name;

        public int Excute(CommandArgs args)
        {
            throw new NotImplementedException();
        }
    }

}
