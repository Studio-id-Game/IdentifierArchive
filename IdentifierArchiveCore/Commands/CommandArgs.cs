namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandArgs
    {
        private static readonly List<Func<CommandArg>> definedArgs = [];

        static CommandArgs()
        {
            Use<CommandArgTargetFolder>();
            Use<CommandArgSettingsFolder>();
            Use<CommandArgIdentifier>();
            Use<CommandArgUserName>();
            Use<CommandArgAutoFolderCreate>();
            Use<CommandArgAutoFileOverwrite>();
            Use<CommandArgAutoIdentifierIncrement>();
            Use<CommandArgUnSafe>();
        }

        public static void Use<T>() where T : CommandArg, new()
        {
            foreach (var arg in definedArgs)
            {
                var a = arg();
                if (a is T) return;
            }

            definedArgs.Add(() => new T());
        }
        
        private readonly CommandArg[] commandArgs;
        
        public CommandArgs()
        {
            commandArgs = definedArgs.Select(e => e()).ToArray();
        }

        public CommandArgs(ReadOnlySpan<CommandArg> sauce)
        {
            commandArgs = new CommandArg[sauce.Length];
            for (int i = 0; i < sauce.Length; i++)
            {
                commandArgs[i] = sauce[i].Copy();
            }
        }


        public T? Get<T>() where T : CommandArg
        {
            foreach (var arg in commandArgs)
            {
                if(arg is T ret) return ret; 
            }

            return null;
        }

        public string? GetText<T>() where T : CommandArg
        {
            return Get<T>()?.ValueText;
        }

        public bool? GetFlag<T>() where T : CommandArg
        {
            return Get<T>()?.ValueFlag;
        }

        public string? TargetFolder
        {
            get => GetText<CommandArgTargetFolder>();
            set => Get<CommandArgTargetFolder>()!.ValueText = value;
        }

        public string? SettingsFolder
        {
            get => GetText<CommandArgSettingsFolder>();
            set => Get<CommandArgSettingsFolder>()!.ValueText = value;
        }

        public string? Identifier
        {
            get => GetText<CommandArgIdentifier>();
            set => Get<CommandArgIdentifier>()!.ValueText = value;
        }

        public string? UserName
        {
            get => GetText<CommandArgUserName>();
            set => Get<CommandArgUserName>()!.ValueText = value;
        }

        public bool? AutoFolderCreate
        {
            get => GetFlag<CommandArgAutoFolderCreate>();
        }

        public bool? AutoFileOverwrite
        {
            get => GetFlag<CommandArgAutoFileOverwrite>();
        }

        public bool? AutoIdentifierIncrement
        {
            get => GetFlag<CommandArgAutoIdentifierIncrement>();
        }

        public bool UnSafe
        {
            get => GetReaded<CommandArgUnSafe>();
        }

        public bool GetReaded<T>() where T : CommandArg
        {
            return Get<T>()?.IsReaded ?? false;
        }

        public bool TryRead(ReadOnlySpan<string> args, out ReadOnlySpan<string> next)
        {
            next = [];

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                bool readed = false;
                foreach (var commandArg in commandArgs)
                {
                    if (commandArg.TryRead(arg))
                    {
                        readed = true;
                        break;
                    }
                }

                if (!readed)
                {
                    if (arg.Trim(' ', '\n', '\r', '\t').StartsWith('-'))
                    {
                        using (new UseConsoleColor(ConsoleColor.Red))
                        {
                            Console.WriteLine($"Unknown arg. ({arg})");
                        }
                        return false;
                    }
                    else
                    {
                        next = args[i..];
                        break;
                    }
                }
            }

            return true;
        }

        public bool CheckRequire(CommandAction master, bool settingsFodler = false, bool targetFolder = false, bool identifier = false)
        {
            List<string> needs = [];

            if (settingsFodler && !CheckRequire<CommandArgSettingsFolder>(out var sfArg, arg => arg.ValueText == null))
            {
                needs.Add($"{sfArg?.Name} ({sfArg?.ArgID})");
            }

            if (targetFolder && !CheckRequire<CommandArgTargetFolder>(out var tfArg, arg => arg.ValueText == null))
            {
                needs.Add($"{tfArg?.Name} ({tfArg?.ArgID})");
            }

            if (identifier && !CheckRequire<CommandArgIdentifier>(out var idArg, arg => arg.ValueText == null))
            {
                needs.Add($"{idArg?.Name} ({idArg?.ArgID})");
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

        public bool CheckRequire<T>(out T? arg, Func<T, bool> check) where T : CommandArg
        {
            arg = Get<T>();
            return arg != null && check(arg);
        }

        public CommandArgs Copy()
        {
            return new(commandArgs);
        }

        public override string ToString()
        {
            return string.Join(' ', commandArgs as object[]);
        }
    }
}
