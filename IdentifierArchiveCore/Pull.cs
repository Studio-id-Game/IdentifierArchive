namespace IdentifierArchiveCore
{
    public sealed class Pull : ActionBase
    {
        public override string Command => Commands.PULL;

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