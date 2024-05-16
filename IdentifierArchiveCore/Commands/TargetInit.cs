namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class TargetInit : ICommandAction
    {
        public const string CommandID = "ti";

        public const string Name = "Target-Init";

        string ICommandAction.CommandID => CommandID;

        string ICommandAction.Name => Name;

        public int Excute(CommandArgs args)
        {
            throw new NotImplementedException();
        }
    }

}
