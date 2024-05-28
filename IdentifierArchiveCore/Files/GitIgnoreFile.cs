using System.Runtime.Serialization;

namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public sealed class GitignoreFile(GitignoreFileType type) : TextFileObject<GitignoreFile>
    {
        public const string FileNameStatic = ".gitignore";

        public static string GetScreenName(GitignoreFileType type)
        {
            return type switch
            {
                GitignoreFileType.Target => "Target folder .gitignore",
                GitignoreFileType.Zip => "Zip folder .gitignore",
                _ => ".gitignore"
            };
        }

        public static string GetText(GitignoreFileType type)
        {
            return type switch
            {
                GitignoreFileType.Target => string.Join('\n', "*", $"!/{FileNameStatic}", $"!/{IdentifierFile.GetFileName(IdentifierFileType.Archive)}"),
                GitignoreFileType.Zip => string.Join('\n', "*", $"!/{FileNameStatic}"),
                _ => ""
            };
        }

        public GitignoreFileType Type { get; } = type;

        public GitignoreFile() : this(GitignoreFileType.None) { }

        public override string Text { get; set; } = GetText(type);

        public override string ScreenName => GetScreenName(Type);

        public override string FileName => FileNameStatic;
    }
}