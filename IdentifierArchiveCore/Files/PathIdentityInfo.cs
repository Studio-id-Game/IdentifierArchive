namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public class PathIdentityInfo<T> where T : FileSystemInfo
    {
        public T Info { get; }
        public string RelativePath { get; }
        public DirectoryInfo ParentInfo { get; }
        public string? RelativeParentPath { get; }
        public string Name => Info.Name;

        public PathIdentityInfo(T path, DirectoryInfo basePath)
        {
            Info = path;
            RelativePath = Path.GetRelativePath(basePath.FullName, path.FullName);
            var parent =
                path is FileInfo fi ? fi.Directory :
                path is DirectoryInfo di ? di.Parent :
                null;

            ParentInfo = parent!;
            RelativeParentPath = parent == null ? null : Path.GetRelativePath(parent.FullName, basePath.FullName);
        }

        public string Replace(string target, PathIdentity identity)
        {
            return target.Replace(identity.Name, Name)
                .Replace(identity.Path, RelativePath)
                .Replace(identity.Parent, RelativeParentPath)
                .Replace(identity.PathAbsolute, Info.FullName)
                .Replace(identity.ParentAbsolute, ParentInfo!.FullName);
        }
    }
}