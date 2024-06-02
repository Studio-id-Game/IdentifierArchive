namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public abstract class CommandArg
    {
        public CommandArg() { }

        public CommandArg(CommandArg sauce)
        {
            ValueText = sauce.ValueText;
            ValueFlag = sauce.ValueFlag;
            IsReaded = sauce.IsReaded;
        }

        public abstract string Name { get; }

        public abstract string ArgID { get; }

        public string? Value { get; set; } = "";

        public virtual string? ValueText
        {
            get => string.IsNullOrWhiteSpace(Value) ? null : Value;
            set
            {
                Value = value;
            }
        }

        public virtual bool? ValueFlag
        {
            get => Value switch
            {
                "true" or null => true,
                "false" or "!" => false,
                "null" or "" or _ => null,
            };
            set
            {
                Value = value switch
                {
                    true => null,
                    false => "!",
                    null => "",
                };
            }
        }

        public bool IsReaded { get; private set; }

        public bool TryRead(string arg)
        {
            var argSp = arg.Split('=');
            var inputArgName = argSp[0].Trim(' ', '\n', '\r', '\t');

            var readed = StringComparer.Ordinal.Equals(inputArgName, ArgID);

            if (readed)
            {
                Value = argSp.ElementAtOrDefault(1);
            }

            IsReaded |= readed;
            return readed;
        }

        public abstract CommandArg Copy();

        public override string ToString()
        {
            return ArgID;
        }

        public string ToStringAsText()
        {
            if (ValueText == null)
            {
                return $"{ArgID}=(Empty)";
            }
            else
            {
                return $"{ArgID}=\"{ValueText}\"";
            }
        }

        public string ToStringAsFlag()
        {
            return $"{ArgID}={ValueFlag?.ToString() ?? "(Default)"}";
        }

        public string ToStringAsReaded()
        {
            return $"{ArgID}={IsReaded}";
        }
    }
}
