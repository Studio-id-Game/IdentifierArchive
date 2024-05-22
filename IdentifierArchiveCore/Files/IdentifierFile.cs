using System.Runtime.Serialization;

namespace StudioIdGames.IdentifierArchiveCore.Files
{

    public sealed class IdentifierFile(IdentifierFileType type) : TextFileObject<IdentifierFile>
    {
        public static string GetScreenName(IdentifierFileType type)
        {
            return type switch
            {
                IdentifierFileType.Archive => "ArchiveIdentifier",
                IdentifierFileType.Current => "CurrentIdentifier",
                _ => "Identifier"
            };
        }

        public static string GetFileName(IdentifierFileType type)
        {
            return type switch
            {
                IdentifierFileType.Archive => "IdentifierArchive.identifier",
                IdentifierFileType.Current => "IdentifierArchiveCurrent.identifier",
                _ => ".identifier"
            };
        }

        public IdentifierFile() : this(IdentifierFileType.None) { }

        public IdentifierFileType Type { get; set; } = type;

        public override string Text { get; set; } = "DEFAULT_IDENTIFIER";

        public override string ScreenName => GetScreenName(Type);

        public override string FileName => GetFileName(Type);
    }
}