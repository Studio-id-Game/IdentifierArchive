﻿using System;
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
            using (new UseConsoleColor(ConsoleColor.Blue))
            {
                Console.WriteLine($"Excute: {this}");
            }
            using (new UseConsoleColor(ConsoleColor.DarkGray))
            {
                Console.WriteLine($"Args: {args}\n");
            }
            return -1;
        }

        public override string ToString()
        {
            return $"{Name} ({CommandID})";
        }
    }
}
