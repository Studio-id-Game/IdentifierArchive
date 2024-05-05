using StudioIdGames.IdentifierArchiveCore.Files;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace StudioIdGames.IdentifierArchiveCore
{
    public sealed partial class Push : ActionBase
    {
        public override string Command => Commands.PUSH;

        public override ActionInfo? Excute(ReadOnlySpan<string> args)
        {
            var initRes = new Init().Excute(args[1..]);

            if (initRes != null && initRes.IsError)
            {
                return initRes;
            }

            if (args.Length < 2)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = "引数の数が足りません"
                };
            }

            var settingsFilePath = args[0];
            var settingFileInfo = new FileInfo($"{settingsFilePath}/{SettingsFile.FileName}");
            var localKeyFileInfo = new FileInfo($"{settingsFilePath}/{LocalKeyFile.FileName}");
            var targetPath = args[1];
            var targetDirectoryInfo = new DirectoryInfo(targetPath);
            var identifierFileInfo = new FileInfo($"{targetDirectoryInfo.FullName}/{IdentifierFile.ArchiveFileName}");
            var currentIdentifierFileInfo = new FileInfo($"{targetDirectoryInfo.FullName}/{IdentifierFile.CurrentFileName}");

            if (!settingFileInfo.Exists)
            {
                return new ActionInfo()
                {
                    IsError = true,
                    Message = $"Settings File が見つかりません。({settingFileInfo.FullName})"
                };
            }


            LocalKeyFile.Data? localKey = null;
            if (localKeyFileInfo.Exists)
            {
                localKey = LocalKeyFile.FromBytes(File.ReadAllBytes(localKeyFileInfo.FullName));
            }


            var newIdentifier = args.Length > 2 ? FixIdentifier(args[2], 0) : FixIdentifier(File.ReadAllText(identifierFileInfo.FullName), 1);
            File.WriteAllText(identifierFileInfo.FullName, newIdentifier);
            File.WriteAllText(currentIdentifierFileInfo.FullName, newIdentifier);

            var setting = SettingsFile.FromBytes(File.ReadAllBytes(settingFileInfo.FullName));

            setting.SetEnvironment(targetPath: targetDirectoryInfo.FullName,
                targetName: targetDirectoryInfo.Name,
                identifier: newIdentifier,
                localKey: localKey);

            var workSpaceDirectoryInfo = new DirectoryInfo(setting.WorkSpace);

            if (!workSpaceDirectoryInfo.Exists)
            {
                workSpaceDirectoryInfo.Create();
            }

            ProcessStartInfo process_start_info = new()
            {
                FileName = "cmd",
                Arguments = "/c " + setting.ZipCommand,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            Process? process = Process.Start(process_start_info);
            if (process != null)
            {
                string res = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
                Console.WriteLine(res);
            }

            return null;
        }

        static string FixIdentifier(string identifier, int add)
        {
            // 正規表現を使って文字列の末尾にある数字を検出
            var regex = FindNumRegex();
            var match = regex.Match(identifier);

            if (match.Success)
            {
                // 末尾にある数字を取得
                string numberString = match.Value;

                // 数字を整数に変換してインクリメント
                int number = int.Parse(numberString);
                number+= add;

                // インクリメントした数字を5桁に0詰めしてフォーマット
                string newNumberString = number.ToString("D5");

                // 末尾の数字を新しい数字に置き換える
                return string.Concat(identifier.AsSpan(0, match.Index), newNumberString);
            }
            else
            {
                // 末尾に "_00000" を追加
                return identifier + "_00000";
            }
        }

        [GeneratedRegex(@"(\d+)$")]
        private static partial Regex FindNumRegex();
    }
}