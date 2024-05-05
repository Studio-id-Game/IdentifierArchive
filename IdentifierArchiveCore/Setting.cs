using System;
using System.Runtime.InteropServices;
using Utf8Json;
using StudioIdGames.IdentifierArchiveCore.Files;

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
            if (args.Length < 2)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = "引数の数が足りません"
                };
            }

            var subCommnad = args[0];
            var directoryPath = args[1];
            var directoryInfo = new DirectoryInfo(directoryPath);

            return subCommnad switch
            {
                SubCommands.CREATE => Cteate(directoryInfo),
                SubCommands.READ => Read(directoryInfo),
                _ => new ActionInfo()
                {
                    IsError = true,
                    Message = "Unknown command."
                },
            };
        }

        public static ActionInfo Cteate(DirectoryInfo directoryInfo)
        {
            if (!directoryInfo.Exists)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = " Directory is not exists."
                };
            }

            var fileInfo = new FileInfo($"{directoryInfo.FullName}/{SettingsFile.FileName}");

            if (!fileInfo.Exists)
            {
                File.WriteAllBytes(fileInfo.FullName, SettingsFile.DefaultValue);
            }

            return new ActionInfo()
            {
                Message = $"Setting file is created. ({fileInfo.FullName})"
            };
        }

        public static ActionInfo Read(DirectoryInfo directoryInfo)
        {
            if (!directoryInfo.Exists)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = $" Directory is not exists. ({directoryInfo.FullName})"
                };
            }

            var fileInfo = new FileInfo($"{directoryInfo.FullName}/{SettingsFile.FileName}");

            if (fileInfo.Exists)
            {
                var text = File.ReadAllBytes(fileInfo.FullName);
                var data = SettingsFile.FromBytes(text);

                return new ActionInfo()
                {
                    Message = $"Settings : \n{JsonSerializer.PrettyPrint(data.ToBytes())}\n",
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