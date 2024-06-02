using StudioIdGames.IdentifierArchiveCore.Commands;
using StudioIdGames.IdentifierArchiveCore.Files;

namespace StudioIdGames.IdentifierArchiveCore.FolderControllers
{
    public class ZipFolderController : FolderController
    {
        public const string BackupIdentifier = "IdentifierArchiveSystem_AutoBackup";
        private readonly SettingsFile settingsWithOutIdentifier;

        public ZipFolderController(SettingsFile settingsWithOutIdentifier)
        {
            this.settingsWithOutIdentifier = new();
            this.settingsWithOutIdentifier.CopyFrom(settingsWithOutIdentifier);
        }

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

        private bool GitignoreSetup()
        {
            return Gitignore.ToFile(FolderInfo, out var created, out _, true, false) || created;
        }

        public int CreateZipFile(IdentifierFile identifier, CommandArgs args)
        {
            var settings = settingsWithOutIdentifier.GetReplaced(identifier: identifier.Text);
            var userInfo = new UserInfoFile().FromFile(settingsWithOutIdentifier.GetLocalkeyFolderInfo());

            if (userInfo == null) return -1;

            if (!FolderSetup(args))
            {
                return -1;
            }

            var zipFileInfo = settings.GetZipFileInfo();
            if (zipFileInfo.Exists)
            {
                Console.WriteLine($"File is exists. ({zipFileInfo.FullName})");

                if (ConsoleUtility.Question("Overwrite this file?", args.AutoFileOverwrite))
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

                    if (!ConsoleUtility.Question($"Increment identifier? ({oldIdentifier} to {identifier.Text})", args.AutoIdentifierIncrement))
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
                if(!CheckUser(metaFileData, userInfo))
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

        public void DeleteZipFile(List<string> unsafeFileNames, bool? autoFileOverwrite, string? identifier)
        {
            var allFiles = GetAllFiles()
                ?.OrderBy(f => f.LastAccessTimeUtc)
                ?.Where(file => file.Extension.TrimStart('.') == settingsWithOutIdentifier.ZipExtension.TrimStart('.'))
                ?.ToArray() ?? [];

            if (allFiles.Length > 0)
            {
                if (allFiles.Length <= 2)
                {
                    unsafeFileNames.AddRange(allFiles.Select(file => file.FullName));
                }
                else if (allFiles.Length > 0)
                {
                    var lastAccess = allFiles.Last().LastAccessTimeUtc;
                    var saveTime = lastAccess - TimeSpan.FromDays(7);
                    Console.WriteLine($"Last access time : UTC {lastAccess} (Files before UTC {saveTime} are automatically deleted.)\n");
                    var lastSaveIndex = Array.FindIndex(allFiles, file => file.LastAccessTimeUtc >= saveTime);
                    var saveFiles = allFiles[lastSaveIndex..];

                    unsafeFileNames.AddRange(saveFiles.Select(file => file.FullName));
                }
            }

            if (string.IsNullOrWhiteSpace(identifier))
            {
                int count = 0;
                foreach (var file in allFiles)
                {
                    identifier = Path.GetFileNameWithoutExtension(file.Name);

                    var info = settingsWithOutIdentifier.GetZipFileInfo(identifier);
                    var ret = ConsoleUtility.DeleteFile(info, unsafeFileNames, autoFileOverwrite);
                    if (ret)
                    {
                        var metaFile = new UserInfoFile() { CustomFileName = identifier };
                        ConsoleUtility.DeleteFile(metaFile.GetFileInfo(FolderInfo), [], true);
                        count++;
                    }
                }

                Console.WriteLine($"Delete \"{count}\" {settingsWithOutIdentifier.ZipExtension} and meta files. ");
            }
            else
            {
                DeleteZipFile(unsafeFileNames, autoFileOverwrite, identifier);
            }

        }

        public int UploadZipFile(string identifier)
        {
            var settings = settingsWithOutIdentifier.GetReplaced(identifier: identifier);
            var localKeyFolderController = new LocalKeyFolderController(settings);

            var zipMetaFileInfo = GetZipMetaFileInfo(identifier);
            if (!zipMetaFileInfo.Exists)
            {
                Console.WriteLine($"Zip meta file does not exist. ({zipMetaFileInfo.FullName})\n");
                return -1;
            }

            var zipMetaFile = new UserInfoFile() { CustomFileName = identifier };
            zipMetaFile = zipMetaFile.FromFile(zipMetaFileInfo)!;

            var userInfo = localKeyFolderController.GetUserInfo();
            if (userInfo == null)
            {
                Console.WriteLine($"User info file does not exist. ({zipMetaFileInfo.FullName})\n");
                return -1;
            }

            if (!CheckUser(zipMetaFile, userInfo))
            {
                return -1;
            }

            var zipFileInfo = GetZipFileInfo(identifier);
            if (!zipFileInfo.Exists)
            {
                Console.WriteLine($"Zip file does not exist. ({zipFileInfo.FullName})\n");
                return -1;
            }

            var metaULExit = settings.ExcuteUpload(zipMetaFileInfo);

            if (metaULExit < 0)
            {
                Console.WriteLine("Zip meta file Upload Command failed.\n");
                return metaULExit;
            }

            Console.WriteLine("Zip meta file Upload Command complete.\n");

            var zipULExit = settings.ExcuteUpload(zipFileInfo);

            if (zipULExit < 0)
            {
                Console.WriteLine("Zip file Upload Command failed.\n");
                return zipULExit;
            }

            Console.WriteLine("Zipfile Upload Command complete.\n");

            return 0;
        }

        public int DownloadZipFile(string identifier, bool? autoFileOverwrite)
        {
            var settings = settingsWithOutIdentifier.GetReplaced(identifier: identifier);

            var zipMetaFileInfo = GetZipMetaFileInfo(identifier);
            var zipFileInfo = GetZipFileInfo(identifier);

            var metaExist = ConsoleUtility.CheckFile(zipMetaFileInfo, "Zip meta", out var metaCreated, out var metaOverwrited, true, autoFileOverwrite, (info) =>
            {
                settings.ExcuteDownload(info);
            });

            if (!metaCreated && !(metaExist && metaOverwrited))
            {
                return -1;
            }

            var exist = ConsoleUtility.CheckFile(zipFileInfo, "Zip", out var created, out var overwrited, true, metaOverwrited, (info) =>
            {
                settings.ExcuteDownload(info);
            });

            return 0;
        }

        private static bool CheckUser(UserInfoFile zipMeta, UserInfoFile userInfo)
        {
            Console.WriteLine($"UserName: {userInfo.UserName}, UserID: {userInfo.UserID}");
            Console.WriteLine($"File owner UserName: {zipMeta.UserName}, File owner UserID: {zipMeta.UserID}\n");

            if (zipMeta.UserID == userInfo.UserID)
            {
                return true;
            }
            else
            {
                using (new UseConsoleColor(ConsoleColor.Red))
                {
                    Console.WriteLine($"You can not edit or upload this file. (you can edit only your created archive.)");
                }

                return false;
            }
        }

    }
}