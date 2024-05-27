using StudioIdGames.IdentifierArchiveCore.Commands;
using StudioIdGames.IdentifierArchiveCore.Files;
using System.Security.Cryptography.X509Certificates;

namespace StudioIdGames.IdentifierArchiveCore.FolderControllers
{
    public class ZipFolderController(SettingsFile settingsWithOutIdentifier) : FolderController
    {
        public const string BackupIdentifier = "IdentifierArchiveSystem_AutoBackup";

        public override string ScreenName => "ZipArchive";

        private static GitignoreFile Gitignore => new(GitignoreFileType.Zip);

        public override DirectoryInfo FolderInfo => settingsWithOutIdentifier.GetZipFolderInfo();

        public override bool FolderSetup(CommandArgs args)
        {
            var ret = base.FolderSetup(args) && GitignoreSetup();
            FolderSetupEnd(ret);
            return ret;
        }

        public FileInfo GetZipFileInfo(string identifier) => settingsWithOutIdentifier.GetZipFileInfo(identifier);

        public FileInfo GetZipMetaFileInfo(string identifier)
        {
            var file = new UserInfoFile() { CustomFileName = identifier };
            return file.GetFileInfo(FolderInfo);
        }

        public UserInfoFile? GetZipMetaFile(string identifier)
        {
            var file = new UserInfoFile() { CustomFileName = identifier };
            return file.FromFile(FolderInfo);
        }

        public int CreateZipFile(IdentifierFile identifier, bool? autoFileOverwrite, bool? autoIdentifierIncrement)
        {
            var settings = settingsWithOutIdentifier.GetReplaced(identifier: identifier.Text);
            var userInfo = new UserInfoFile().FromFile(settingsWithOutIdentifier.GetLocalkeyFolderInfo());

            if (userInfo == null) return -1;

            var zipFileInfo = settings.GetZipFileInfo();
            if (zipFileInfo.Exists)
            {
                Console.WriteLine($"File is exists. ({zipFileInfo.FullName})");

                if (ConsoleUtility.Question("Overwrite this file?", autoFileOverwrite))
                {
                    ConsoleUtility.DeleteFile(zipFileInfo, [], true);
                }
                else
                {
                    var oldIdentifier = identifier.Text;
                    if (settingsWithOutIdentifier == null)
                    {
                        return -1;
                    }

                    while (zipFileInfo.Exists)
                    {
                        identifier.FixIdentifier(1);
                        settings = settingsWithOutIdentifier.GetReplaced(identifier: identifier.Text);
                        zipFileInfo = settings.GetZipFileInfo();
                    }

                    if (!ConsoleUtility.Question($"Increment identifier? ({oldIdentifier} to {identifier.Text})", autoIdentifierIncrement))
                    {
                        return -1;
                    }
                }
            }

            Console.WriteLine($"Create zip meta file. ({zipFileInfo.FullName})");

            var metaFile = new UserInfoFile() { CustomFileName = settings.Identifier! };
            metaFile.CopyFrom(userInfo);

            if(metaFile.ToFile(FolderInfo, out _, out _, true, false))
            {
                var metaFileData = metaFile.FromFile(FolderInfo)!;
                if(metaFileData.UserID != userInfo.UserID)
                {
                    Console.WriteLine($"UserName: {userInfo.UserName}, UserID: {userInfo.UserID}");
                    Console.WriteLine($"File owner UserName: {metaFileData.UserName}, File owner UserID: {metaFileData.UserID}");

                    using (new UseConsoleColor(ConsoleColor.Red))
                    {
                        Console.WriteLine($"You can not edit this file. (you can edit only your created archive.)");
                    }
                    return -1;
                }
            }

            Console.WriteLine($"Create zip file. ({zipFileInfo.FullName})");

            var ret = settings.ExcuteZip();

            if (ret < 0)
            {
                Console.WriteLine("Zip Command failed.\n");
                return ret;
            }


            Console.WriteLine("Zip Command complete.\n");

            return 0;
        }

        public bool DeleteCache(IEnumerable<string> unsafeFileNames, bool? autoFileOverwrite, string identifier)
        {
            var info = settingsWithOutIdentifier.GetZipFileInfo(identifier);
            var ret = ConsoleUtility.DeleteFile(info, unsafeFileNames, autoFileOverwrite);
            if (ret)
            {
                var metaFile = new UserInfoFile() { CustomFileName = identifier };
                ConsoleUtility.DeleteFile(metaFile.GetFileInfo(FolderInfo), [], true);
            }
            return ret;
        }

        private bool GitignoreSetup()
        {
            return Gitignore.ToFile(FolderInfo, out var created, out _, true, false) || created;
        }
    }
}