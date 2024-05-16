using System.Collections.ObjectModel;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public class CommandActions : ICommandAction
    {
        private static readonly ReadOnlyDictionary<string, ICommandAction> DefinedActions = new(new Dictionary<string, ICommandAction>()
        {
            [ZipAdd.CommandID] = new ZipAdd(),
            [ZipExtract.CommandID] = new ZipExtract(),
        });

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
                if (DefinedActions.TryGetValue(arg, out ICommandAction? action) && action != null)
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
