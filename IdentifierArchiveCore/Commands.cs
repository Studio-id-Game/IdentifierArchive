namespace StudioIdGames.IdentifierArchiveCore
{
    public static class Commands
    {
        public const string PUSH = "push";
        public const string PULL = "pull";
        public const string SETTING = "setting";
        public const string INIT = "init";

        public static bool Check(string command)
        {
            var list = new string[] { PUSH, PULL, SETTING, INIT };
            return list.Any(s => StringComparer.Ordinal.Equals(command, s));
        }
    }
}