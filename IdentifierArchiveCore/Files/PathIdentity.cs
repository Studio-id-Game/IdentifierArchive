using System.IO;

namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public readonly struct PathIdentity(string baseName)
    {
        public string Name { get; } = $"%{baseName}_Name%";
        public string Path { get; } = $"%{baseName}%";
        public string Parent { get; } = $"%{baseName}_PARENT%";
        public string PathAbsolute { get; } = $"%{baseName}_ABS%";
        public string ParentAbsolute { get; } = $"%{baseName}_PARENT_ABS%";
    }
}