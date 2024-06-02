using System.Diagnostics;
using System.Text;
using Utf8Json;

namespace NugetUtility
{
}

namespace StudioIdGames.DevTools.LicenseHunter
{
    public class SettingsFile
    {
        public class TargetInfo
        {
            public string ProjectDirectory { get; set; } = "Project directory from directory of settings file";
            public string ExportPath { get; set; } = "Export path from ProjectDirectory";
            public string ManualPackageInformationPath = "Manual package information path from ProjectDirectory";
        }

        public TargetInfo[] Targets { get; set; } = [new TargetInfo()];
        public string[] IgnoreLicenseType { get; set; } = ["Ignore license type"];
    }

    internal class Program
    {
        private const string DotnetProjectLicenses = "dotnet-project-licenses";
        private const string TARGET = "%TARGET%";

        // ターゲットフォルダの指定をできるようにする

        static readonly DirectoryInfo ExportDirInfo = new("./exports");
        static readonly DirectoryInfo LicenseDirectoryInfo = new($"{ExportDirInfo.FullName}/licenses/");
        static readonly FileInfo ListJsonFileInfo = new($"{ExportDirInfo.FullName}/output.json");
        static readonly string CommandExportLicense = $"-i {TARGET} -t -f {LicenseDirectoryInfo.FullName} --export-license-texts --convert-html-to-text --use-project-assets-json";
        static readonly string CommandExportListJson = $"-i {TARGET} -o -j -t --outfile {ListJsonFileInfo.FullName} --use-project-assets-json";

        private static async Task<int> Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("DevTools.LicenseHunter is installed.");
                return 0;
            }

            var command = args[0];
            var settingsPath = args.Length < 2 ? "./DevTools.LicenseHunterSettings.json" : args[1];

            try
            {
                switch (command)
                {
                    case "init":
                        return Init(settingsPath);
                    case "e":
                    case "export":
                        return await Export(settingsPath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error : {e.Message}"); ;
                return -1;
            }


            Console.WriteLine("Unknown Command.");
            return -1;
        }

        private static int Init(string settingsPath)
        {
            var settingsFileInfo = new FileInfo(settingsPath);
            SettingsFile settingsFile;
            if (settingsFileInfo.Exists)
            {
                settingsFile = JsonSerializer.Deserialize<SettingsFile>(File.ReadAllBytes(settingsFileInfo.FullName));

                foreach (var target in settingsFile.Targets)
                {
                    var mpiPath = target.ManualPackageInformationPath;
                    if (!string.IsNullOrEmpty(mpiPath))
                    {
                        var mpiFileInfo = new FileInfo($"{target.ProjectDirectory}/{mpiPath}");
                        if (!mpiFileInfo.Exists)
                        {
                            var bytes = JsonSerializer.Serialize<NugetUtility.LibraryInfo[]>([new NugetUtility.LibraryInfo()]);
                            File.WriteAllBytes(mpiFileInfo.FullName, JsonSerializer.PrettyPrintByteArray(bytes));
                        }
                    }
                }
            }
            else
            {
                settingsFile = new SettingsFile();
                File.WriteAllBytes(settingsFileInfo.FullName, JsonSerializer.PrettyPrintByteArray(JsonSerializer.Serialize(settingsFile)));
            }

            return 0;
        }

        private static async Task<int> Export(string settingsPath)
        {
            var settingsFileInfo = new FileInfo(settingsPath);

            if (!settingsFileInfo.Exists)
            {
                Console.WriteLine($"Settings file is not exist. ({settingsFileInfo.FullName})");
                return -1;
            }

            var settingsFile = JsonSerializer.Deserialize<SettingsFile>(File.ReadAllBytes(settingsFileInfo.FullName));


            foreach (var target in settingsFile.Targets)
            {
                var projectDirInfo = new DirectoryInfo($"{settingsFileInfo.Directory!.FullName}/{target.ProjectDirectory}");
                var projectExportDirInfo = new DirectoryInfo($"{settingsFileInfo.Directory!.FullName}/{target.ExportPath}");
                var projectManualPackageInformationFileInfo = new FileInfo($"{settingsFileInfo.Directory!.FullName}/{target.ManualPackageInformationPath}");

                Console.WriteLine($"Target : ({projectDirInfo.FullName})\n");

                Console.WriteLine($"Initialize export folder ({ExportDirInfo.FullName})\n");
                if (ExportDirInfo.Exists)
                {
                    /*
                    foreach(var file in ExportDirInfo.GetFiles("*", SearchOption.AllDirectories))
                    {
                        file.Delete();
                    }
                    */
                }
                else
                {
                    ExportDirInfo.Create();
                }

                if (ListJsonFileInfo.Exists)
                {
                    ListJsonFileInfo.Delete();
                }

                var opt = "";

                if (!string.IsNullOrEmpty(target.ManualPackageInformationPath) && projectManualPackageInformationFileInfo.Exists)
                {
                    opt += $"--manual-package-information {projectManualPackageInformationFileInfo.FullName}";
                }

                if (projectDirInfo.Exists)
                {
                    var commandExportLicense = $"{CommandExportLicense.Replace(TARGET, projectDirInfo.FullName)} {opt}";
                    var commandExportListJson = $"{CommandExportListJson.Replace(TARGET, projectDirInfo.FullName)} {opt}";

                    Console.WriteLine($"Excute : {DotnetProjectLicenses} {commandExportListJson}");
                    var commandExportListJsonRes = await ExcuteCommandAsync(DotnetProjectLicenses, commandExportListJson);
                    Console.WriteLine($"ExitCode:{commandExportListJsonRes}");
                    if (commandExportListJsonRes < 0) return -1;

                    var listJsonFile = File.ReadAllBytes(ListJsonFileInfo.FullName);
                    File.WriteAllBytes(ListJsonFileInfo.FullName, JsonSerializer.PrettyPrintByteArray(listJsonFile));
                    var listJson = JsonSerializer.Deserialize<NugetUtility.LibraryInfo[]>(listJsonFile);

                    bool update = false;
                    var outputList = new List<NugetUtility.LibraryInfo>();

                    Console.WriteLine("LICENSES :");
                    foreach (var item in listJson)
                    {
                        if (settingsFile.IgnoreLicenseType.Contains(item.LicenseType))
                        {
                            /*
                            var licenseFileInfo = new FileInfo($"{LicenseDirectoryInfo.FullName}\\{item.PackageName}_{item.PackageVersion}.txt");
                            if (licenseFileInfo.Exists)
                            {
                                licenseFileInfo.Delete();
                            }
                            */
                        }
                        else
                        {
                            Console.WriteLine($"\t{item.PackageName}_{item.PackageVersion} | {item.LicenseType}");
                            if (outputList.All(e => e.PackageName != item.PackageName || e.PackageVersion != item.PackageVersion))
                            {
                                outputList.Add(item);
                                var licenseFileInfo = new FileInfo($"{LicenseDirectoryInfo.FullName}\\{item.PackageName}_{item.PackageVersion}.txt");
                                if (!update && !licenseFileInfo.Exists)
                                {
                                    update = true;
                                }
                            }
                        }
                    }

                    if (update)
                    {
                        Console.WriteLine("Need Update Licenses.\n");
                        var commandExportLicenseRes = await ExcuteCommandAsync(DotnetProjectLicenses, commandExportLicense);
                        if (commandExportLicenseRes < 0) return -1;

                        foreach (var item in listJson)
                        {
                            if (settingsFile.IgnoreLicenseType.Contains(item.LicenseType))
                            {
                                var licenseFileInfo = new FileInfo($"{LicenseDirectoryInfo.FullName}\\{item.PackageName}_{item.PackageVersion}.txt");
                                if (licenseFileInfo.Exists)
                                {
                                    licenseFileInfo.Delete();
                                }
                            }
                        }
                    }

                    var outputTextBuilder = new StringBuilder("THIRD PARTY NOTICES\n\n");
                    foreach (var item in outputList)
                    {
                        const string Devider = "################################################################################";

                        var licenseFileInfo = new FileInfo($"{LicenseDirectoryInfo.FullName}\\{item.PackageName}_{item.PackageVersion}.txt");
                        if (licenseFileInfo.Exists)
                        {
                            outputTextBuilder.AppendLine(Devider);
                            outputTextBuilder.AppendLine();
                            outputTextBuilder.AppendLine();

                            outputTextBuilder.AppendLine($"- PackageName : {item.PackageName}");
                            outputTextBuilder.AppendLine($"- PackageVersion : {item.PackageVersion}");
                            outputTextBuilder.AppendLine($"- PackageUrl : {item.PackageUrl}");
                            outputTextBuilder.AppendLine($"- Copyright : {item.Copyright}");
                            outputTextBuilder.AppendLine();
                            outputTextBuilder.AppendLine("- License : \n");
                            outputTextBuilder.AppendLine(File.ReadAllText(licenseFileInfo.FullName));

                            outputTextBuilder.AppendLine();
                            outputTextBuilder.AppendLine();
                        }
                    }

                    File.WriteAllText($"{projectExportDirInfo.FullName}/THIRD_PARTY_NOTICES.txt", outputTextBuilder.ToString());
                }


                Console.WriteLine($"\nTarget Complete : ({projectDirInfo.FullName})\n");
            }


            return 0;
        }

        public static async Task<int> ExcuteCommandAsync(string filename, string command)
        {
            ProcessStartInfo process_start_info = new()
            {
                FileName = filename,
                Arguments = $"/c {command}",
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
                        Console.Error.WriteLine(line);
                    }

                    await process.WaitForExitAsync();
                    return process.ExitCode;
                }

                return -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}