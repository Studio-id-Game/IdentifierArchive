using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;

namespace StudioIdGames.GoogleDriveStrage
{
    internal class Program
    {
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
            if (args.Length < 3)
            {
                Console.WriteLine("Unkown Command.");
                return;
            }

            var strageRootFolderID = args[0];
            var subFolderName = args[1];
            var filePath = args[2];

            string[] scopes = [DriveService.Scope.Drive];

            GoogleCredential credential;
            try
            {
                credential = GoogleCredential.FromFile("GoogleDriveStrageLocalKey~").CreateScoped(scopes);
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

            using FileStream fsu = new(filePath, FileMode.Open);
            try
            {
                var list = service.Files.List().Execute();
                foreach ( var file in list.Files)
                {
                    if (file.Parents.Contains(rootFolderID))
                    {
                        file.
                    }
                }

                Google.Apis.Drive.v3.Data.File meta = new()
                {
                    Name = Path.GetFileName(filePath),
                    Parents = [folderID],
                    
                };
                var req = service.Files.Create(meta, fsu, null);
                req.Fields = "id, name";
                req.Upload();
            }
            finally
            {
                fsu.Close();
            }
        }

        private static void Download(ReadOnlySpan<string> args)
        {

        }
    }
}