﻿using StudioIdGames.IdentifierArchiveCore.Files;
using System.Diagnostics;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace StudioIdGames.IdentifierArchiveCore
{
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
                    Console.Write($"{text} (y|n) > ");
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

        public static bool CheckFile(FileInfo info, string screenName)
        {
            return CheckFileSystem(info, $"{screenName} file");
        }

        public static bool CheckFile(FileInfo info, string screenName, out bool created, out bool overwrited, bool? autoCreate = false, bool? autoOverwrite = false, Action<FileInfo>? onCreate = null)
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
                            var oldFileName = info.FullName;
                            var backupFileName = info.FullName + ".bk";
                            Console.WriteLine($"Backup old {screenName} file. ({backupFileName}).");
                            File.Delete(backupFileName);
                            info.MoveTo(backupFileName);
                            info = new FileInfo(oldFileName);

                            Console.WriteLine($"{screenName} file overwriting...");
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

            return exists; 
        }

        public static bool CheckFolder(DirectoryInfo info, string screenName)
        {
            return CheckFileSystem(info, $"{screenName} folder");
        }

        public static bool CheckFolder(DirectoryInfo info, string screenName, out bool created, bool? autoCreate = false)
        {
            return CheckFileSystem(info, $"{screenName} folder", out created, autoCreate, e => e.Create());
        }

        public static bool CheckFileSystem<T>(T info, string screenName) where T : FileSystemInfo
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
            var exists = CheckFileSystem(info, screenName);
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

        public static void WriteError(Exception error)
        {
            Console.Error.WriteLine($"ERROR : {error}\n");
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