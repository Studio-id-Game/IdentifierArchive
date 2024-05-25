using System.ComponentModel.Design;

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

        public virtual string? ValueText { get; set; }

        public virtual bool? ValueFlag { get; set; }    

        public bool IsReaded { get; private set; }

        public bool TryRead(string arg)
        {
            var argSp = arg.Split('=');
            var inputArgName = argSp[0].Trim(' ', '\n', '\r', '\t');

            var readed = StringComparer.Ordinal.Equals(inputArgName, ArgID);
            
            if (readed)
            {
                var value = argSp.ElementAtOrDefault(1);
                ValueText = string.IsNullOrWhiteSpace(value) ? null : value;
                ValueFlag = value switch
                {
                    "true" or null => true,
                    "false" or "!" => false,
                    "null" or "" or _ => null,
                };
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
