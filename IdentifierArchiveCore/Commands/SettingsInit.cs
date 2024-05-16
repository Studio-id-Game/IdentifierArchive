namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class SettingsInit : ICommandAction
    {
        public const string CommandID = "si";

        public const string Name = "Settings-Init";

        string ICommandAction.CommandID => CommandID;

        string ICommandAction.Name => Name;

        public int Excute(CommandArgs args)
        {
            throw new NotImplementedException();
        }
    }

}
