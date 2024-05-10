using System.Diagnostics;
using System.Text.RegularExpressions;

namespace StudioIdGames.IdentifierArchiveCore
{
    public partial class Utility
    {
        public static int ExcuteCommand(string command)
        {
            ProcessStartInfo process_start_info = new()
            {
                FileName = "cmd",
                Arguments = "/c " + command,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true, 
                RedirectStandardError = true,
            };

            try
            {
                using var process = Process.Start(process_start_info);
                if (process != null)
                {
                    // 標準出力を読み取り、Consoleに表示
                    while (!process.StandardOutput.EndOfStream)
                    {
                        string? line = process.StandardOutput.ReadLine();
                        Console.WriteLine(line);
                    }

                    // 標準エラーを読み取り、Consoleに表示
                    while (!process.StandardError.EndOfStream)
                    {
                        string? line = process.StandardError.ReadLine();
                        Console.WriteLine(line);
                    }

                    process.WaitForExit();
                    return process.ExitCode;
                }

                return -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static string FixIdentifier(string identifier, int add)
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
                number += add;

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

        public static void ConsoleDeleteLine()
        {
            int prePos = Console.CursorLeft;//現在カーソル位置を取得
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("".PadRight(prePos));//前のカーソル位置まで空白埋めする
            Console.SetCursorPosition(0, Console.CursorTop);
        }
    }
}