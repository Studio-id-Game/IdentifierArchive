using Microsoft.VisualBasic.FileIO;
using StudioIdGames.IdentifierArchiveCore.Commands;
using StudioIdGames.IdentifierArchiveCore.Files;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace StudioIdGames.IdentifierArchiveCore
{
    public readonly ref struct UseConsoleColor
    {
        private readonly ConsoleColor prev;

        public UseConsoleColor(ConsoleColor color)
        {
            prev = Console.ForegroundColor;
            Console.ForegroundColor = color;
        }

        public void Dispose()
        {
            Console.ForegroundColor = prev;
        }
    }

    public static class ConsoleUtility
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
                bool hasError = false;
                if (process != null)
                {
                    while (!process.StandardOutput.EndOfStream)
                    {
                        string? line = process.StandardOutput.ReadLine();
                        Console.WriteLine(line);
                    }

                    while (!process.StandardError.EndOfStream)
                    {
                        string? line = process.StandardError.ReadLine();
                        Console.Error.WriteLine(line);
                        hasError = true;
                    }

                    process.WaitForExit(100000);

                    return hasError ? -1 : process.ExitCode;
                }

                return -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static string QuestionText(string text, string? autoAns)
        {
            if (autoAns != null)
            {
                return autoAns;
            }
            else
            {
                using (new UseConsoleColor(ConsoleColor.Yellow))
                {
                    Console.WriteLine($"{text}");
                    Console.Write($"> ");
                }

                return Console.ReadLine() ?? "";
            }
        }

        public static bool Question(string text, bool? autoAns)
        {
            if (autoAns.HasValue)
            {
                return autoAns.Value;
            }
            else
            {
                while (true)
                {
                    using (new UseConsoleColor(ConsoleColor.Yellow))
                    {
                        Console.WriteLine($"{text}");
                        Console.Write($"(y|n) > ");
                    }
                    var key = Console.ReadKey().Key;
                    if (key == ConsoleKey.Y)
                    {
                        Console.WriteLine("\n");
                        return true;
                    }
                    if (key == ConsoleKey.N)
                    {
                        Console.WriteLine("\n");
                        return false;
                    }

                    DeleteLine();
                }
            }
        }

        public static bool CheckFile(FileInfo info)
        {
            return CheckFileSystem(info);
        }

        public static bool CheckFile(FileInfo info, string screenName, out bool created, out bool overwrited, bool? autoCreate = false, bool? autoOverwrite = false, Action<FileInfo>? onCreate = null, bool backupToRecycleBin = true)
        {
            overwrited = false;
            var exists = CheckFileSystem(info, $"{screenName} file", out created, autoCreate, onCreate);
            if (exists)
            {
                if (autoOverwrite == false)
                {
                    Console.WriteLine($"{screenName} file is already exists. ({info.FullName})\n");
                }
                else
                {
                    if (Question($"Overwrite old {screenName} file?", autoOverwrite))
                    {
                        try
                        {
                            Console.WriteLine($"{screenName} file overwriting...");

                            if (backupToRecycleBin)
                            {
                                DeleteFile(info, [], true);
                            }

                            onCreate?.Invoke(info);
                            Console.WriteLine($"{screenName} file overwrited.\n");
                            overwrited = true;
                        }
                        catch (Exception error)
                        {
                            WriteError(error);
                            Console.WriteLine($"{screenName} file overwrite failed.\n");
                        }
                    }
                }
            }

            Console.WriteLine();

            return exists; 
        }

        public static bool CheckFolder(DirectoryInfo info)
        {
            return CheckFileSystem(info);
        }

        public static bool CheckFolder(DirectoryInfo info, string screenName, out bool created, bool? autoCreate = false)
        {
            return CheckFileSystem(info, $"{screenName} folder", out created, autoCreate, e => e.Create());
        }

        public static bool CheckFileSystem<T>(T info) where T : FileSystemInfo
        {
            if (info.Exists)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckFileSystem<T>(T info, string screenName, out bool created, bool? autoCreate = false, Action<T>? onCreate = null) where T : FileSystemInfo
        {
            created = false;
            var exists = CheckFileSystem(info);
            if (exists)
            {
                return true;
            }
            else
            {
                if (Question($"Create new {screenName}?", autoCreate))
                {
                    try
                    {
                        Console.WriteLine($"{screenName} creating...");
                        onCreate?.Invoke(info);
                        Console.WriteLine($"{screenName} created.\n");
                        created = true;
                    }
                    catch (Exception e)
                    {
                        WriteError(e);
                        Console.WriteLine($"{screenName} creation failed.\n");
                    }
                }

                return false;
            }
        }

        public static bool DeleteFile(FileInfo info, IEnumerable<string> unsafeFileNames, bool? autoFileOverwrite)
        {
            bool deleted = false;

            Console.WriteLine($"Deleting file... ({info.FullName})");

            if (info != null && info.Exists)
            {
                if (unsafeFileNames.Contains(info.FullName))
                {
                    if (Question("We recommend not deleting this file for safety reasons. Do you want to delete it?", autoFileOverwrite))
                    {
                        FileSystem.DeleteFile(info.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                        deleted = true;
                    }
                }
                else
                {
                    FileSystem.DeleteFile(info.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    deleted = true;
                }
            }
            else
            {

                Console.WriteLine($"File does not exist.");
            }

            if (deleted)
            {
                Console.WriteLine($"File deleted. (send to recycle bin)\n");
            }
            else
            {
                Console.WriteLine($"File not deleted.\n");
            }

            return deleted;
        }

        public static void WriteError(Exception error)
        {
            using (new UseConsoleColor(ConsoleColor.Red))
            {
                Console.Error.Write($"ERROR : ");
            }
            Console.Error.WriteLine($"{error}\n");
        }

        public static void Need(string master, string needType, params string[] needs)
        {
            if (needs.Length == 0) return;
            if (needs.Length == 1)
            {
                
                Console.WriteLine($"{master} needs {needs[0]} {needType}.");
            }
            else
            {
                Console.WriteLine($"{master} needs all the {needType}(s) {string.Join(", ", needs)}.");
            }
        }

        public static void DeleteLine()
        {
            int prePos = Console.CursorLeft;//現在カーソル位置を取得
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("".PadRight(prePos));//前のカーソル位置まで空白埋めする
            Console.SetCursorPosition(0, Console.CursorTop);
        }

    }
}