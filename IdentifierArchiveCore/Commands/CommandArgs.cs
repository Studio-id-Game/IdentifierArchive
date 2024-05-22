using System;
using System.ComponentModel.Design;
using System.Xml.Linq;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandArgs
    {
        public const string TargetFolderArgName = "-tf";
        public const string SettingsFolderArgName = "-sf";
        public const string IdentifierArgName = "-id";
        public const string AutoFolderCreateArgName = "-auto-folder-create";
        public const string AutoFileOverwriteArgName = "-auto-file-overwrite";

        private static bool TryReadString(string argNameCheck, string argName, string? argValue, out bool isEmpty)
        {
            isEmpty = false;
            if (argName == argNameCheck)
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

        private static bool TryReadFlag(string argNameCheck, string argName, string? argValue, out bool? value)
        {
            value = null;
            if (argName == argNameCheck)
            {
                switch (argValue)
                {
                    case "true":
                    case null: value = true; break;
                    case "false":
                    case "!": value = false; break;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public string TargetFolder { get; set; } = string.Empty;
        public string SettingsFolder { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
        public bool? AutoFolderCreate { get; set; } = null;
        public bool? AutoFileOverwrite { get; set; } = null;

        public bool TryRead(ReadOnlySpan<string> args, out ReadOnlySpan<string> next)
        {
            next = [];

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var argSp = arg.Split('=');
                var argName = argSp[0].Trim(' ', '\n', '\r', '\t');
                var argValue = argSp.ElementAtOrDefault(1);

                if (TryReadString(TargetFolderArgName, argName, argValue, out bool isEmpty))
                {
                    if (isEmpty)
                    {
                        Console.WriteLine($"Value of arg ({TargetFolderArgName}) is empty.");
                        return false;
                    }

                    TargetFolder = argValue!;
                }
                else if (TryReadString(SettingsFolderArgName, argName, argValue, out isEmpty))
                {
                    if (isEmpty)
                    {
                        Console.WriteLine($"Value of arg ({SettingsFolderArgName}) is empty.");
                        return false;
                    }

                    SettingsFolder = argValue!;
                }
                else if (TryReadString(IdentifierArgName, argName, argValue, out isEmpty))
                {
                    Identifier = isEmpty ? "" : argValue ?? "";
                }
                else if (TryReadFlag(AutoFolderCreateArgName, argName, argValue, out var flagValue))
                {
                    AutoFolderCreate = flagValue;
                }
                else if (TryReadFlag(AutoFileOverwriteArgName, argName, argValue, out flagValue))
                {
                    AutoFileOverwrite = flagValue;
                }
                else if (argName.StartsWith('-'))
                {
                    Console.WriteLine($"Unknown arg. ({argName})");
                    return false;
                }
                else
                {
                    next = args[i..];
                    break;
                }
            }

            return true;
        }

        public bool CheckRequire(CommandAction master, bool targetFolder = false, bool settingsFodler = false, bool identifier = false)
        {
            List<string> needs = [];

            if (targetFolder & string.IsNullOrWhiteSpace(TargetFolder))
            {
                needs.Add(TargetFolderArgName);
            }

            if (settingsFodler & string.IsNullOrWhiteSpace(SettingsFolder))
            {
                needs.Add(SettingsFolderArgName);
            }

            if (identifier & string.IsNullOrWhiteSpace(Identifier))
            {
                needs.Add(IdentifierArgName);
            }

            if (needs.Count > 0)
            {
                ConsoleUtility.Need($"{master.Name} ({master.CommandID})", "augument", [.. needs]);
                return false;
            }
            else
            {
                return true;
            }
        } 

        public CommandArgs Copy()
        {
            return new() 
            {
                SettingsFolder = SettingsFolder, 
                TargetFolder = TargetFolder,
                Identifier = Identifier,
                AutoFileOverwrite = AutoFileOverwrite, 
                AutoFolderCreate = AutoFolderCreate, 
            };
        }

        public override string ToString()
        {
            return $"{SettingsFolderArgName}={SettingsFolder}" +
                $" {TargetFolderArgName}={TargetFolder}" +
                $" {IdentifierArgName}={Identifier}" +
                $" {AutoFolderCreateArgName}={AutoFolderCreate}" +
                $" {AutoFileOverwriteArgName}={AutoFileOverwrite}";
        }
    }
}
