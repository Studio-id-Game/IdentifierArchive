using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace StudioIdGames.IdentifierArchiveCore.Commands
{
    public interface ICommandAction
    {
        public string CommandID { get; }
        public string Name { get; }
        public int Excute(CommandArgs args);
    }
}
