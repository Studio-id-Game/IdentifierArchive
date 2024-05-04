using System;
using System.Runtime.InteropServices;
using Utf8Json;

namespace IdentifierArchiveCore
{
    public sealed class Setting : ActionBase
    {
        static class SubCommands
        {
            public const string CREATE= "create";
            public const string READ = "read";
        }

        public struct SettingData
        {
            public static SettingData FromBytes(byte[] bytes)
            {
                return JsonSerializer.Deserialize<SettingData>(bytes);
            }

            public SettingData()
            {
            }

            public string WorkSpace { get; set; } = "作業フォルダ";
            public string ZipCommand { get; set; } = "暗号圧縮コマンド";
            public string UnzipCommand { get; set; } = "復号解凍コマンド";
            public string UploadCommand { get; set; } = "アップロードコマンド";
            public string DownloadCommand { get; set; } = "ダウンロードコマンド";

            readonly public byte[] ToBytes()
            {
                return JsonSerializer.PrettyPrintByteArray(JsonSerializer.Serialize(this));
            }

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
            var fileInfo = new FileInfo($"{directoryInfo.FullName}/identifierArchive.settings.json");

            return subCommnad switch
            {
                SubCommands.CREATE => Cteate(directoryInfo, fileInfo),
                SubCommands.READ => Read(directoryInfo, fileInfo),
                _ => new ActionInfo()
                {
                    IsError = true,
                    Message = "Unknown command."
                },
            };
        }

        private static ActionInfo Cteate(DirectoryInfo directoryInfo, FileInfo fileInfo)
        {
            if (!directoryInfo.Exists)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = " Directory is not exists."
                };
            }

            if (!fileInfo.Exists)
            {
                File.WriteAllBytes(fileInfo.FullName, new SettingData().ToBytes());
            }

            var identifierFileInfo = new FileInfo($"{directoryInfo.FullName}/.identifier");
            if (!identifierFileInfo.Exists)
            {
                File.WriteAllText(identifierFileInfo.FullName, "identifier");
            }

            var currentIdentifierFileInfo = new FileInfo($"{directoryInfo.FullName}/current.identifier");
            if (!currentIdentifierFileInfo.Exists)
            {
                File.WriteAllText(currentIdentifierFileInfo.FullName, "current identifier");
            }

            var ignoreFileInfo = new FileInfo($"{directoryInfo.FullName}/.gitignore");
            if (!ignoreFileInfo.Exists)
            {
                File.WriteAllText(ignoreFileInfo.FullName, "*\n!.gitignore\n!.identifier");
            }

            return new ActionInfo()
            {
                Message = $"Setting file is created. ({fileInfo.FullName})"
            };
        }

        private static ActionInfo Read(DirectoryInfo directoryInfo, FileInfo fileInfo)
        {
            if (!directoryInfo.Exists)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = $" Directory is not exists. ({directoryInfo.FullName})"
                };
            }

            if (!fileInfo.Exists)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = $"File is not exists. ({fileInfo.FullName})"
                };
            }

            var text = File.ReadAllBytes(fileInfo.FullName);
            var data = SettingData.FromBytes(text);

            return new ActionInfo()
            {
                Message = $"Settings : \n{JsonSerializer.PrettyPrint(data.ToBytes())}\n",
            };
        }

    }
}