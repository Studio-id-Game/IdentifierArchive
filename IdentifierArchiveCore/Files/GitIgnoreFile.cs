namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public class GitIgnoreFile : TextFile
    {
        public const string FileName = ".gitignore";
        public const string ValueInTargetFolder = "*\n!.gitignore\n!.identifier";
        public static bool ToFileInTargetFolder(FileInfo fileInfo)
        {
            return ToFile(fileInfo, ValueInTargetFolder);
        }
    }
}