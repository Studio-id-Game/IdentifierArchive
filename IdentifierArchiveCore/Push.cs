namespace IdentifierArchiveCore
{
    public sealed class Push : ActionBase
    {
        public override string Command => Commands.PUSH;

        public override ActionInfo? Excute(ReadOnlySpan<string> args)
        {
            if (args.Length < 2)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = "引数の数が足りません"
                };
            }
            return null;
        }
    }
}