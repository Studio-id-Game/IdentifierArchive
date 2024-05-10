using StudioIdGames.IdentifierArchiveCore.Files;

namespace StudioIdGames.IdentifierArchiveCore
{
    public sealed class Init : ActionBase
    {
        public override string Command => Commands.INIT;

        public override ActionInfo? Excute(ReadOnlySpan<string> args)
        {
            if (args.Length < 2)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = "引数の数が足りません"
                };
            }

            var settingsFolderPath = args[0];
            var targetFolderPath = args[1];

            TargetFolderController controller = new(settingsFolderPath, targetFolderPath);

            var fileCheck = controller.CheckSettingsFileAndTargetFolder();
            if(fileCheck != null)
            {
                return fileCheck;
            }

            controller.CreateInitFiles();

            return null;
        }
    }
}