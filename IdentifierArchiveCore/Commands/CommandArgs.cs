namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandArgs
    {
        public const string TargetFolderArgName = "-tf";
        public const string SettingsFolderArgName = "-sf";
        public const string IdentifierArgName = "-id";
        public const string QuestionDefaultYesArgName = "-y";
        public const string QuestionDefaultNoArgName = "-n";
        public const string QuestionDefaultNullArgName = "-q";

        public string TargetFolder { get; private set; } = string.Empty;
        public string SettingsFolder { get; private set; } = string.Empty;
        public string? Identifier { get; private set; } = null;
        public bool? QuestionDefault { get; private set; } = null;

        public bool TryRead(ReadOnlySpan<string> args, out ReadOnlySpan<string> next)
        {
            next = [];

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var argSp = arg.Split('=');
                var argName = argSp[0].Trim(' ', '\n', '\r', '\t');
                var argValue = argSp.ElementAtOrDefault(1);
                bool isEmpty;

                if (TryReadString(TargetFolderArgName, argName, argValue, out isEmpty))
                {
                    if(isEmpty)
                    {
                        Console.WriteLine($"Value of {TargetFolderArgName} is empty.");
                        return false;
                    }

                    TargetFolder = argValue!;
                }
                else if (TryReadString(SettingsFolderArgName, argName, argValue, out isEmpty))
                {
                    if (isEmpty)
                    {
                        Console.WriteLine($"Value of {SettingsFolderArgName} is empty.");
                        return false;
                    }

                    SettingsFolder = argValue!;
                }
                else if (TryReadString(IdentifierArgName, argName, argValue, out isEmpty))
                {
                    Identifier = isEmpty ? null : argValue;
                }
                else if (TryReadFlag(QuestionDefaultYesArgName, argName))
                {
                    QuestionDefault = true;
                }
                else if (TryReadFlag(QuestionDefaultNoArgName, argName))
                {
                    QuestionDefault = false;
                }
                else if (TryReadFlag(QuestionDefaultNullArgName, argName))
                {
                    QuestionDefault = null;
                }else
                {
                    next = args[i..];
                    return true;
                }
            }

            return true;
        }

        private static bool TryReadString(string argNameCheck, string argName, string? argValue, out bool isEmpty)
        {
            isEmpty = false;
            if (TryReadFlag(argNameCheck, argName))
            {
                if (string.IsNullOrWhiteSpace(argValue))
                {
                    isEmpty = true;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool TryReadFlag(string argNameCheck, string argName)
        {
            return argName == argNameCheck;
        }
    }
}
