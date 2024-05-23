using System.IO;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StudioIdGames.IdentifierArchiveCore.Files
{

    public abstract class TextFileObject<TSelf> : FileObject<TextFileController<TSelf>, TSelf>
        where TSelf : TextFileObject<TSelf>, new()
    {
        public static Encoding DefaultEncoding { get; protected set; } = Encoding.UTF8;

        public abstract string Text { get; set; }

        public override void CopyFrom(TSelf other)
        {
            Text = other.Text;
        }

        public override string AsText()
        {
            return Text;
        }
    }
}