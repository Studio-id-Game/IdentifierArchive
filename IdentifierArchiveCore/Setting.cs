using System;
using System.Runtime.InteropServices;
using Utf8Json;
using StudioIdGames.IdentifierArchiveCore.Files;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace StudioIdGames.IdentifierArchiveCore
{
    public sealed class Setting : ActionBase
    {
        static class SubCommands
        {
            public const string CREATE = "create";
            public const string READ = "read";
        }

        public override string Command => Commands.SETTING;

        public override ActionInfo? Excute(ReadOnlySpan<string> args)
        {
            if (args.Length < 1)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = "引数の数が足りません"
                };
            }

            var subCommnad = args[0];
            var folderPath = args[1];

            return subCommnad switch
            {
                SubCommands.CREATE => Cteate(folderPath),
                SubCommands.READ => Read(folderPath),
                _ => new ActionInfo()
                {
                    IsError = true,
                    Message = "Unknown command."
                },
            };
        }

        public static ActionInfo Cteate(string folderPath)
        {
            var folderInfo = new DirectoryInfo(folderPath);
            if (!folderInfo.Exists)
            {
                Console.WriteLine($"Folder is not Exists. ({folderInfo.FullName})\nCreate new folder? (Y|N)");
                var key = Console.ReadKey();
                if(key.Key == ConsoleKey.Y)
                {
                    folderInfo.Create();
                }
                else
                {
                    return new ActionInfo()
                    {
                        IsError = true,
                        Message = $"Folder is not created."
                    };
                }
            }

            var fileInfo = new FileInfo($"{folderInfo.FullName}/{SettingsFile.FileName}");

            if (fileInfo.Exists)
            {
                return new ActionInfo()
                {
                    Message = $"Setting file is exist. ({fileInfo.FullName})"
                };
            }
            else
            {
                if (SettingsFile.CreateDefaultFile(fileInfo))
                {
                    var sampleKeyFileInfo = new FileInfo($"{folderInfo.FullName}/SampleLocalKey.{LocalKeyFile.FileName}");
                    LocalKeyFile.CreateDefaultFile(sampleKeyFileInfo);

                    return new ActionInfo()
                    {
                        Message = $"Setting file is created. ({fileInfo.FullName})"
                    };
                }
                else
                {
                    return new ActionInfo()
                    {
                        IsError = true,
                        Message = $"Can not create setting file. ({fileInfo.FullName})"
                    };
                }
            }
        }

        public static ActionInfo Read(string folderPath)
        {
            var folderInfo = new DirectoryInfo(folderPath);
            if (!folderInfo.Exists)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = $" Directory is not exists. ({folderInfo.FullName})"
                };
            }

            var fileInfo = new FileInfo($"{folderInfo.FullName}/{SettingsFile.FileName}");
            var settings =  SettingsFile.FromFile(fileInfo);
            if (settings != null)
            {
                var localkeyFiles = new DirectoryInfo(settings.LocalkeyFolderAbsolute).GetFiles("*", SearchOption.AllDirectories);
                var localkeyFilePaths = localkeyFiles.Select(e => Path.GetRelativePath(settings.LocalkeyFolderAbsolute, e.FullName)).ToArray();
                var controller = new TargetFolderController(folderPath, "/SAMPLE_TARGET/SAMPLE_TARGET_SUBFOLDER/");
                controller.TryLoad("SAMPLE_IDENTIFIER", true);

                Console.WriteLine($"Settings : \n{JsonSerializer.PrettyPrint(settings.ToBytes())}\n");
                Console.WriteLine($"Settings with PathIdentity: \n{JsonSerializer.PrettyPrint(controller.Settings.ToBytes())}\n");
                Console.WriteLine($"LocalkeyFiles : \n\t{string.Join("\n\t", localkeyFilePaths)}\n");
                return new ActionInfo()
                {
                    Message = "File is exists.",
                };
            }
            else
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = $"File is not exists. ({fileInfo.FullName})"
                };
            }

        }

    }
}