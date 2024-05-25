﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Xml.Linq;
using static StudioIdGames.IdentifierArchiveCore.Commands.CommandArgs;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandArgs
    {
        public class ArgTargetFolder : CommandArg
        {
            public ArgTargetFolder() { }

            public ArgTargetFolder(ArgTargetFolder sauce) : base(sauce) { }

            public override string Name => $"-{nameof(ArgTargetFolder)}";

            public override string ArgID => "-tf";

            public override CommandArg Copy()
            {
                return new ArgTargetFolder(this);
            }

            public override string ToString()
            {
                return ToStringAsText();
            }
        }

        public class ArgSettingsFolder : CommandArg
        {
            public ArgSettingsFolder() { }

            public ArgSettingsFolder(ArgSettingsFolder sauce) : base(sauce) { }

            public override string Name => $"-{nameof(ArgSettingsFolder)}";

            public override string ArgID => "-sf";

            public override CommandArg Copy()
            {
                return new ArgSettingsFolder(this);
            }
            public override string ToString()
            {
                return ToStringAsText();
            }
        }

        public class ArgIdentifier : CommandArg
        {
            public ArgIdentifier() { }

            public ArgIdentifier(ArgIdentifier sauce) : base(sauce) { }

            public override string Name => $"-{nameof(ArgIdentifier)}";

            public override string ArgID => "-id";

            public override CommandArg Copy()
            {
                return new ArgIdentifier(this);
            }
            public override string ToString()
            {
                return ToStringAsText();
            }
        }

        public class ArgAutoFolderCreate : CommandArg
        {
            public ArgAutoFolderCreate() { }

            public ArgAutoFolderCreate(ArgAutoFolderCreate sauce) : base(sauce) { }

            public override string Name => $"-{nameof(ArgAutoFolderCreate)}";

            public override string ArgID => "-auto-folder-c";

            public override CommandArg Copy()
            {
                return new ArgAutoFolderCreate(this);
            }

            public override string ToString()
            {
                return ToStringAsFlag();
            }
        }

        public class ArgAutoFileOverwrite : CommandArg
        {
            public ArgAutoFileOverwrite() { }

            public ArgAutoFileOverwrite(ArgAutoFileOverwrite sauce) : base(sauce) { }

            public override string Name => $"-{nameof(ArgAutoFileOverwrite)}";

            public override string ArgID => "-auto-file-ow";

            public override CommandArg Copy()
            {
                return new ArgAutoFileOverwrite(this);
            }

            public override string ToString()
            {
                return ToStringAsFlag();
            }
        }

        public class ArgAutoIdentifierIncrement : CommandArg
        {
            public ArgAutoIdentifierIncrement() { }

            public ArgAutoIdentifierIncrement(ArgAutoIdentifierIncrement sauce) : base(sauce) { }

            public override string Name => $"-{nameof(ArgAutoIdentifierIncrement)}";

            public override string ArgID => "-auto-id-incr";

            public override CommandArg Copy()
            {
                return new ArgAutoIdentifierIncrement(this);
            }

            public override string ToString()
            {
                return ToStringAsFlag();
            }
        }

        public class ArgUnSafe : CommandArg
        {
            public ArgUnSafe() { }

            public ArgUnSafe(ArgUnSafe sauce) : base(sauce) { }

            public override string? ValueText => throw new NotSupportedException();

            public override bool? ValueFlag => throw new NotSupportedException();

            public override string Name => "-UN-SAFE";

            public override string ArgID => "-UN-SAFE";

            public override CommandArg Copy()
            {
                return new ArgUnSafe(this);
            }

            public override string ToString()
            {
                return ToStringAsReaded();
            }
        }
        
        private static readonly List<Func<CommandArg>> definedArgs = [];

        static CommandArgs()
        {
            AddCommand<ArgTargetFolder>();
            AddCommand<ArgSettingsFolder>();
            AddCommand<ArgIdentifier>();
            AddCommand<ArgAutoFolderCreate>();
            AddCommand<ArgAutoFileOverwrite>();
            AddCommand<ArgUnSafe>();
            AddCommand<ArgAutoIdentifierIncrement>();
        }

        public static void AddCommand<T>() where T : CommandArg, new()
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
            get => GetText<ArgTargetFolder>();
            set => Get<ArgTargetFolder>()!.ValueText = value;
        }

        public string? SettingsFolder
        {
            get => GetText<ArgSettingsFolder>();
            set => Get<ArgSettingsFolder>()!.ValueText = value;
        }
        
        public string? Identifier
        {
            get => GetText<ArgIdentifier>();
            set => Get<ArgIdentifier>()!.ValueText = value;
        }
        
        public bool? AutoFolderCreate
        {
            get => GetFlag<ArgAutoFolderCreate>();
        }

        public bool? AutoFileOverwrite
        {
            get => GetFlag<ArgAutoFileOverwrite>();
        }

        public bool? AutoIdentifierIncrement
        {
            get => GetFlag<ArgAutoIdentifierIncrement>();
        }

        public bool UnSafe
        {
            get => GetReaded<ArgUnSafe>();
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
                        Console.WriteLine($"Unknown arg. ({arg})");
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

        public bool CheckRequire(CommandAction master, bool targetFolder = false, bool settingsFodler = false, bool identifier = false)
        {
            List<string> needs = [];

            if (NotSet<ArgSettingsFolder>(settingsFodler, out var name))
            {
                needs.Add(name);
            }

            if (NotSet<ArgTargetFolder>(targetFolder, out name))
            {
                needs.Add(name);
            }

            if (NotSet<ArgIdentifier>(identifier, out name))
            {
                needs.Add(name);
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

        public bool NotSet<T>(bool check, out string name) where T : CommandArg, new()
        {
            var arg = new T();
            name = $"{arg.Name} ({arg.ArgID})";
            return check && GetText<ArgTargetFolder>() == null;
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
