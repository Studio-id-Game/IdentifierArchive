using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public abstract class CommandAction
    {
        public abstract string CommandID { get; }
        public abstract string Name { get; }
        public virtual int Excute(CommandArgs args)
        {
            Console.WriteLine($"Excute: {this}");
            Console.WriteLine($"Args: {args}");
            return -1;
        }

        public override string ToString()
        {
            return $"{Name} ({CommandID})";
        }
    }
}
