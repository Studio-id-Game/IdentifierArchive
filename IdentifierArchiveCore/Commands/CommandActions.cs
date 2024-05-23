﻿using System;
using System.Collections.ObjectModel;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandActions : CommandAction
    {
        private static readonly Dictionary<string, CommandAction> definedActions;

        static CommandActions()
        {
            definedActions = [];
            AddCommand(SettingsInit.Instance);
            AddCommand(SettingsView.Instance);
            AddCommand(TargetInit.Instance);
            AddCommand(TargetView.Instance);
            AddCommand(ZipAdd.Instance);
            AddCommand(ZipExtract.Instance);
            AddCommand(ZipClear.Instance);
            AddCommand(ZipView.Instance);
            AddCommand(RemoteUpload.Instance);
            AddCommand(RemoteDownload.Instance);
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
                    Console.WriteLine($"Error in {action}.\n");
                    return -(1 + i);
                }
                else
                {
                    Console.WriteLine($"{action} Complete.\n");
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