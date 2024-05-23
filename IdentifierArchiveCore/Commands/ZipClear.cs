namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class ZipClear : CommandAction
    {
        public static ZipClear Instance { get; } = new ZipClear();

        private ZipClear() { }

        public override string CommandID => "zc";

        public override string Name => "Zip-Clear";

        public override int Excute(CommandArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
