using System.Runtime.InteropServices;

namespace IdentifierArchiveCore
{
    public sealed class Setting : ActionBase
    {
        static class SubCommands
        {
            public const string CREATE= "create";
            public const string READ = "read";
        }

        readonly struct SettingData
        {
            public SettingData()
            {
            }

            public string WorkSpace { get; init; } = "作業フォルダ";
            public string ZipCommand { get; init; } = "暗号圧縮コマンド";
            public string UnzipCommand { get; init; } = "復号解凍コマンド";
            public string UploadCommand { get; init; } = "アップロードコマンド";
            public string DownloadCommand { get; init; } = "ダウンロードコマンド";

            public string ToText()
            {
                return 
                    @$"// 作業フォルダ
{nameof(WorkSpace)} = {WorkSpace}

// 暗号圧縮コマンド
{nameof(ZipCommand)} = {ZipCommand}

// 復号解凍コマンド
{nameof(UnzipCommand)} = {UnzipCommand}

// アップロードコマンド
{nameof(UploadCommand)} = {UploadCommand}

// ダウンロードコマンド
{nameof(DownloadCommand)} = {DownloadCommand}";
            }

            public static SettingData FromText(string text)
            {
                var workSpace = "";
                var zipCommand = "";
                var unzipCommand = "";
                var uploadCommand = "";
                var downloadCommand = "";

                var lines = text.Split('\n');
                foreach(var line in lines)
                {
                    if(string.IsNullOrEmpty(line) || line.StartsWith("//")) continue;

                    var eqIndex = line.IndexOf('=');

                    if(eqIndex == -1) continue;

                    var id = line[..eqIndex].TrimStart(' ').TrimEnd(' ');
                    var value = line[(eqIndex+1)..].TrimStart(' ').TrimEnd(' ');

                    if (id == nameof(WorkSpace))
                    {
                        workSpace = value;
                    }
                    else if (id == nameof(ZipCommand))
                    {
                        zipCommand = value;
                    }
                    else if (id == nameof(UnzipCommand))
                    {
                        unzipCommand = value;
                    }
                    else if (id == nameof(DownloadCommand))
                    {
                        downloadCommand = value;
                    }
                    else if (id == nameof(UploadCommand))
                    {
                        uploadCommand= value;
                    }
                }

                return new()
                {
                    WorkSpace = workSpace,
                    ZipCommand = zipCommand,
                    UnzipCommand = unzipCommand,
                    UploadCommand = uploadCommand,
                    DownloadCommand = downloadCommand,
                };
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
            var fileInfo = new FileInfo($"{directoryInfo.FullName}/identifierArchive.settings.txt");

            switch (subCommnad)
            {
                case SubCommands.CREATE:

                    if (!directoryInfo.Exists)
                    {
                        return new ActionInfo()
                        {
                            IsError = true,
                            Message = " Directory is not exists."
                        };
                    }

                    if (fileInfo.Exists)
                    {
                        return new ActionInfo()
                        {
                            Message = $"File is exists. ({fileInfo.FullName})"
                        };
                    }
                    File.WriteAllText(fileInfo.FullName, new SettingData().ToText());

                    break;
                
                case SubCommands.READ:
                    
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

                    var text = File.ReadAllText(fileInfo.FullName);
                    var data = SettingData.FromText(text);

                    return new ActionInfo()
                    {
                        Message = $"Settings : \n{data.ToText()}".Replace("\n","\n\t"),
                    };

                default:
                    return new ActionInfo()
                    {
                        IsError = true,
                        Message = "Unknown command."
                    };
            }

            return null;
        }
    }
}