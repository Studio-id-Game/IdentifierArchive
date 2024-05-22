using System;
using System.Collections.ObjectModel;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandActions : ICommandAction
    {
        private static readonly Dictionary<string, ICommandAction> definedActions;

        static CommandActions()
        {
            definedActions = [];
            AddCommand(new SettingsInit());
            AddCommand(new SettingsView());
            AddCommand(new TargetInit());
            AddCommand(new TargetView());
            AddCommand(new ZipAdd());
            AddCommand(new ZipExtract());
            AddCommand(new ZipCacheClear());
            AddCommand(new ZipCacheView());
            AddCommand(new RemoteUpload());
            AddCommand(new RemoteDownload());
        }

        public static void AddCommand<T>(T action) where T : ICommandAction
        {
            definedActions.Add(action.Name, action);
        }

        public static int ExcuteAll(ReadOnlySpan<string> args)
        {
            var next = args;
            var comArgs = new CommandArgs();
            var lastRes = 0;
            while (next.Length > 0)
            {
                var comActions = new CommandActions();
                if (!comActions.TryRead(next, out next)) return -1;
                if (!comArgs.TryRead(next, out next)) return -1;

                lastRes = comActions.Excute(comArgs);
                if (lastRes < 0) break;
            }

            return lastRes;
        }

        ICommandAction[] actions = [];

        public ReadOnlySpan<ICommandAction> Actions => actions;

        public string CommandID => string.Join(' ', actions.Select(e => e.CommandID));

        public string Name => $"[{string.Join(", ", actions.Select(e => e.Name))}]";

        public int Excute(CommandArgs args)
        {
            for (int i = 0; i < Actions.Length; i++)
            {
                var action = Actions[i];
                var res = action.Excute(args);
                if (res < 0)
                {
                    Console.WriteLine($"Error in action. ({action.Name})");
                    return -(1 + i);
                }
            }

            return 0;
        }

        public bool TryRead(ReadOnlySpan<string> args, out ReadOnlySpan<string> next)
        {
            List<ICommandAction> newActions = [];
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (definedActions.TryGetValue(arg, out ICommandAction? action) && action != null)
                {
                    newActions.Add(action);
                }
                else if (arg.StartsWith('-'))
                {
                    actions = [.. newActions];
                    next = args[i..];
                    return true;
                }
                else
                {
                    Console.WriteLine($"Unknown command. ({arg})");
                    actions = [];
                    next = [];
                    return false;
                }
            }

            actions = [.. newActions];
            next = [];
            return true;
        }
    }
}