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
            if(args.Length == 0) 
            {
                Console.WriteLine("GoogleDriveStrage is installed.");
                return 0;
            }
            else
            {
                switch(args[0])
                {
                    case "u":
                        return Upload(args.AsSpan()[1..]);
                    case "d":
                        return Download(args.AsSpan()[1..]);
                    default:
                        Console.WriteLine("Unkown Command.");
                        return -1;

                }
            }
        }

        private static int Upload(ReadOnlySpan<string> args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Unkown Command.");
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
                Console.WriteLine("Credential Error : " + e.Message);
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
                Console.WriteLine("Get Folder List Error : " + e.Message);
                return -1;
            }

            if(list.Count == 0)
            {
                try
                {
                    Console.WriteLine($"Create {subFolderName} folder");
                    list = [CreateFolder(service, strageRootFolderID, subFolderName)];
                }
                catch (Exception e)
                {
                    Console.WriteLine("Create Folder Error : " + e.Message);
                    return -1;
                }
            }

            Console.WriteLine($"Upload {Path.GetFileName(filePath)} to {subFolderName} folder ({list.Count})");
            foreach (var folder in list)
            {
                using FileStream file = new(filePath, FileMode.Open);

                CreateOrUpdateFile(service, folder.Id, Path.GetFileName(filePath), file);
            }

            return 0;
        }

        private static int Download(ReadOnlySpan<string> args)
        {
            return 0;
        }

        private static void CreateOrUpdateFile(DriveService service, string parentID, string fileName, FileStream file)
        {
            var listRequest = service.Files.List();
            listRequest.Q = $"'{parentID}' in parents and name='{fileName}' and trashed=false";

            File meta = new()
            {
                Name = fileName,
                Parents = [parentID],
            };

            var list = listRequest.Execute().Files;
            if (list == null || list.Count == 0)
            {
                var req = service.Files.Create(meta, file, null);
                req.Fields = "id, name";
                req.Upload();
            }
            else
            {
                Console.WriteLine($"UPDATE MODE");
                foreach (var item in list)
                {
                    var req = service.Files.Update(meta, item.Id, file, null);
                    req.Fields = "id, name";
                    req.Upload();
                }
            }
        }

        private static File CreateFolder(DriveService service, string parentID, string name)
        {
            var fobj = new File
            {
                Name = name,
                MimeType = MimeTypeGoogleDriveFolder,
                Parents = [parentID]
            };

            FilesResource.CreateRequest req = service.Files.Create(fobj);
            req.Fields = "id, name";
            return req.Execute();
        }

    }
}