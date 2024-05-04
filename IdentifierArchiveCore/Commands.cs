namespace IdentifierArchiveCore
{
    public static class Commands
    {
        public const string PUSH = "push";
        public const string PULL = "pull";
        public const string SETTING = "setting";

        public static bool Check(string command)
        {
            var list = new string[] { PUSH, PULL, SETTING };
            return list.Any(s => StringComparer.Ordinal.Equals(command, s));
        }
    }
}