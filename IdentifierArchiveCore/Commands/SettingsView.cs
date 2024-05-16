namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class SettingsView : ICommandAction
    {
        public const string CommandID = "sv";

        public const string Name = "Settings-View";

        string ICommandAction.CommandID => CommandID;

        string ICommandAction.Name => Name;

        public int Excute(CommandArgs args)
        {
            throw new NotImplementedException();
        }
    }

}
