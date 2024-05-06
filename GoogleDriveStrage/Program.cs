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

        private static void Main(string[] args)
        {
            if(args.Length == 0) 
            {
                Console.WriteLine("GoogleDriveStrage is installed.");
            }
            else
            {
                switch(args[0])
                {
                    case "u":
                        Upload(args.AsSpan()[1..]);
                        break;
                    case "d":
                        Download(args.AsSpan()[1..]);
                        break;
                    default:
                        Console.WriteLine("Unkown Command.");
                        break;

                }
            }
        }

        private static void Upload(ReadOnlySpan<string> args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Unkown Command.");
                return;
            }


            var localKeyPath = args[0];
            var strageRootFolderID = args[1];
            var subFolderName = args[2];
            var filePath = args[3];

            string[] scopes = [DriveService.Scope.Drive];

            GoogleCredential credential;
            try
            {
                credential = GoogleCredential.FromFile(localKeyPath).CreateScoped(scopes);
            }
            catch
            {
                return;
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
            catch (Exception)
            {
                list = [];
            }

            if(list.Count == 0)
            {
                try
                {
                    Console.WriteLine($"Create {subFolderName} folder");
                    list = [CreateFolder(strageRootFolderID, subFolderName)];
                }
                catch (Exception)
                {
                    list = [];
                }
            }

            Console.WriteLine($"Upload to {subFolderName} folder ({list.Count})");
            foreach (var folder in list)
            {
                using FileStream file = new(filePath, FileMode.Open);

                CreateOrUpdateFile(folder.Id, Path.GetFileName(filePath), file);
            }

            File CreateFolder(string parentID, string name)
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

            void CreateOrUpdateFile(string parentID, string fileName, FileStream file)
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
        }

        private static void Download(ReadOnlySpan<string> args)
        {

        }

    }
}