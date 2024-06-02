using System;
using System.Collections.ObjectModel;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandActions : CommandAction
    {
        private static readonly Dictionary<string, CommandAction> definedActions = new(StringComparer.Ordinal);

        static CommandActions()
        {
            AddCommand(CommandActionSettingsInit.Instance);
            AddCommand(CommandActionSettingsView.Instance);
            AddCommand(CommandActionTargetInit.Instance);
            AddCommand(CommandActionTargetView.Instance);
            AddCommand(CommandActionZipAdd.Instance);
            AddCommand(CommandActionZipExtract.Instance);
            AddCommand(CommandActionZipClear.Instance);
            AddCommand(CommandActionZipView.Instance);
            AddCommand(CommandActionRemoteUpload.Instance);
            AddCommand(CommandActionRemoteDownload.Instance);
        }

        public static void AddCommand<T>(T action) where T : CommandAction
        {
            definedActions.Add(action.Name, action);
            definedActions.Add(action.CommandID, action);
        }

        public static int ExcuteAll(ReadOnlySpan<string> args)
        {
            Console.WriteLine();
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

        CommandAction[] actions = [];

        public ReadOnlySpan<CommandAction> Actions => actions;

        public override string CommandID => string.Join(' ', actions.Select(e => e.CommandID));

        public override string Name => $"[{string.Join(", ", actions.Select(e => e.Name))}]";

        public override int Excute(CommandArgs args)
        {
            base.Equals(args);
            for (int i = 0; i < Actions.Length; i++)
            {
                var action = Actions[i];
                var res = action.Excute(args);
                if (res < 0)
                {
                    using (new UseConsoleColor(ConsoleColor.Red))
                    {
                        Console.WriteLine($"Error in {action}.\n");
                    }
                    return -(1 + i);
                }
                else
                {
                    using (new UseConsoleColor(ConsoleColor.Green))
                    {
                        Console.WriteLine($"{action} Complete.\n");
                    }
                }
            }

            return 0;
        }

        public bool TryRead(ReadOnlySpan<string> args, out ReadOnlySpan<string> next)
        {
            List<CommandAction> newActions = [];
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (definedActions.TryGetValue(arg, out CommandAction? action) && action != null)
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
                    using (new UseConsoleColor(ConsoleColor.Red))
                    {
                        Console.WriteLine($"Unknown command. ({arg})");
                    }
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