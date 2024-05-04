namespace IdentifierArchiveCore
{
    public class PullArg
    {
        public string SettingsFilePath { get; init; } = "";
        public string TargetPath { get; init; } = "";
        public string? Identifier { get; init; }
    }
}