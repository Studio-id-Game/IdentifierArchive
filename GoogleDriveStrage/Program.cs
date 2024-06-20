using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using File = Google.Apis.Drive.v3.Data.File;

namespace StudioIdGames.GoogleDriveStrage
{
    internal class Program
    {
        const string MimeTypeGoogleDriveFolder = "application/vnd.google-apps.folder";

        private static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("GoogleDriveStrage is installed.");
                return 0;
            }
            else
            {
                switch (args[0])
                {
                    case "u":
                        return Upload(args.AsSpan()[1..]);
                    case "d":
                        return Download(args.AsSpan()[1..]);
                    default:
                        Console.WriteLine("Unkown Command.\n");
                        return -1;

                }
            }
        }

        private static int Upload(ReadOnlySpan<string> args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Unkown Command.\n");
                return -1;
            }


            var localKeyPath = args[0];
            var strageRootFolderID = args[1];
            var subFolderName = args[2].Replace('\\', '/');
            var filePath = args[3];

            string[] scopes = [DriveService.Scope.Drive];

            GoogleCredential credential;
            try
            {
                credential = GoogleCredential.FromFile(localKeyPath).CreateScoped(scopes);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Credential Error : {e.Message}\n");
                return -1;
            }


            BaseClientService.Initializer init = new()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoogleDriveStrage App"
            };

            DriveService service = new(init);

            var listRequest = service.Files.List();

            listRequest.Q = $"'{strageRootFolderID}' in parents and mimeType='{MimeTypeGoogleDriveFolder}' and name='{subFolderName}' and trashed=false";

            IList<File> list;
            try
            {
                list = listRequest.Execute().Files;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Get Folder List Error : {e.Message}\n");
                return -1;
            }

            if (list.Count == 0)
            {
                try
                {
                    list = [CreateFolder(service, strageRootFolderID, subFolderName)];
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error : {e.Message}\n");
                    return -1;
                }
            }

            Console.WriteLine($"Upload {Path.GetFileName(filePath)} to {subFolderName} folder ({list.Count})");
            foreach (var folder in list)
            {
                using FileStream file = new(filePath, FileMode.Open);

                if (CreateOrUpdateFile(service, folder.Id, Path.GetFileName(filePath), file) < 0)
                {
                    return -1;
                }
            }

            Console.WriteLine($"Upload completed. ({Path.GetFileName(filePath)} to {subFolderName})\n");

            return 0;
        }

        private static int Download(ReadOnlySpan<string> args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Unkown Command.\n");
                return -1;
            }


            var localKeyPath = args[0];
            var strageRootFolderID = args[1];
            var subFolderName = args[2].Replace('\\', '/');
            var filePath = args[3];
            var fileName = Path.GetFileName(filePath);

            string[] scopes = [DriveService.Scope.Drive];

            GoogleCredential credential;
            try
            {
                credential = GoogleCredential.FromFile(localKeyPath).CreateScoped(scopes);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Credential Error : {e.Message}\n");
                return -1;
            }


            BaseClientService.Initializer init = new()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoogleDriveStrage App"
            };

            DriveService service = new(init);

            Console.WriteLine($"Search {subFolderName} folder.");
            var subFolder = GetFileWithName(service, strageRootFolderID, "root", subFolderName, $"and mimeType='{MimeTypeGoogleDriveFolder}'");
            if (subFolder == null) return -1;

            Console.WriteLine($"Download [{subFolderName}]->[{fileName}] to {filePath}");
            var file = GetFileWithName(service, subFolder.Id, subFolderName, fileName);
            if (file == null) return -1;

            var req = service.Files.Get(file.Id);
            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            req.Download(fs);

            Console.WriteLine($"Download completed. ({Path.GetFileName(filePath)} from {subFolderName})\n");
            return 0;
        }

        private static int CreateOrUpdateFile(DriveService service, string parentID, string fileName, FileStream file)
        {
            var listRequest = service.Files.List();
            listRequest.Q = $"'{parentID}' in parents and name='{fileName}' and trashed=false";

            File meta = new()
            {
                Name = fileName,
            };

            var list = listRequest.Execute().Files;

            if (list == null || list.Count == 0)
            {
                meta.Parents = [parentID];

                var req = service.Files.Create(meta, file, null);
                req.Fields = "id, name";
                var res = req.Upload();

                if (res.Status == UploadStatus.Failed)
                {
                    Console.WriteLine($"Error: {res.Exception.Message}");
                    return -1;
                }
            }
            else
            {
                Console.WriteLine($"UPDATE MODE");
                foreach (var oldMeta in list)
                {
                    var req = service.Files.Update(meta, oldMeta.Id, file, null);
                    req.Fields = "id, name";
                    var res = req.Upload();
                    if (res.Status == UploadStatus.Failed)
                    {
                        Console.WriteLine($"Error: {res.Exception.Message}");
                        return -1;
                    }
                }
            }

            return 0;
        }

        private static File CreateFolder(DriveService service, string parentID, string name)
        {
            Console.WriteLine($"Create {name} folder");
            var fobj = new File
            {
                Name = name,
                MimeType = MimeTypeGoogleDriveFolder,
                Parents = [parentID]
            };

            FilesResource.CreateRequest req = service.Files.Create(fobj);
            req.Fields = "id, name";
            var folder = req.Execute();

            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(500);

                var check = service.Files.Get(folder.Id).Execute();
                if (check != null)
                {
                    return folder;
                }

                Console.WriteLine("Wait for folder creation...");

                Thread.Sleep(500);
            }

            throw new TimeoutException("Folder creation timeout or  does not created");
        }

        private static File? GetFileWithName(DriveService service, string parentID, string parentName, string name, string addon = "")
        {
            var getFile = service.Files.List();
            getFile.Q = $"'{parentID}' in parents and name='{name}' and trashed=false {addon}";

            Console.WriteLine($"Search {name} in {parentName} folder.");
            IList<File> list;
            try
            {
                list = getFile.Execute().Files;
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR : {e.Message}\n");
                return null;
            }

            if (list.Count <= 0)
            {
                Console.WriteLine($"Does not exist {name} in {parentName} folder.\n");
                return null;
            }
            else if (list.Count > 1)
            {
                Console.WriteLine($"Meny exists {name} in {parentName} folder. (count = {list.Count})\n");
                return null;
            }

            Console.WriteLine($"Exits {name} in {parentName} folder.\n");

            return list[0];
        }
    }
}