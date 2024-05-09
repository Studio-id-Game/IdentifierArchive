namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public class TextFile
    {
        public static string? FromFile(FileInfo fileInfo)
        {
            try
            {
                if (fileInfo.Exists)
                {
                    return File.ReadAllText(fileInfo.FullName);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool ToFile(FileInfo fileInfo, string value)
        {
            try
            {
                if (fileInfo.Exists)
                {
                    File.WriteAllText(fileInfo.FullName, value);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}