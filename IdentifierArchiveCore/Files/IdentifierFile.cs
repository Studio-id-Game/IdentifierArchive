namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public class IdentifierFile : TextFile
    {
        public const string ArchiveFileName = "IdentifierArchive.identifier";
        public const string CurrentFileName = "IdentifierArchiveCurrent.identifier";
        public const string DefaultValue = "DEFAULT_IDENTIFIER";

        public static bool ToFile(FileInfo fileInfo)
        {
            return ToFile(fileInfo, DefaultValue);
        }
    }
}