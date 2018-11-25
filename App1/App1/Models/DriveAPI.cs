using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using App1.Services;

namespace App1.Models
{
    public class DriveAPI
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        static string[] Scopes = { DriveService.Scope.Drive};

        public DriveAPI()
        {
            ClientSecrets Secrets = new ClientSecrets()
            {
                ClientId = Constants.ClientID,
                ClientSecret = Constants.ClientSecret
            };

            TokenResponse Token = new TokenResponse { RefreshToken = Constants.RefreshToken };

            UserCredential Credential = new UserCredential(new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = Secrets
                }),
                "user",
                Token);
            try
            {
                using (var stream = new FileStream("driveCredentials.json", FileMode.Open, FileAccess.Read))
                {
                    // Create Drive API service.
                    var service = new DriveService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = Credential,
                        ApplicationName = Constants.ApplicationName,
                    });

                    // Define parameters of request.
                    FilesResource.ListRequest getFolder = service.Files.List();
                    getFolder.Q = "mimeType='application/vnd.google-apps.folder' and name='Expenses'";
                    //listRequest.PageSize = 10;
                    getFolder.Fields = "nextPageToken, files(id, name)";
                    getFolder.Spaces = "drive";


                    // get FolderID
                    IList<Google.Apis.Drive.v3.Data.File> folder = getFolder.Execute()
                        .Files;

                    var folderID = folder[0].Id;

                    FilesResource.ListRequest filesInFolder = service.Files.List();
                    filesInFolder.Q = "'" + folderID.ToString() + "' in parents";
                    //listRequest.PageSize = 10;
                    filesInFolder.Fields = "nextPageToken, files(id, name)";
                    filesInFolder.Spaces = "drive";

                    IList<Google.Apis.Drive.v3.Data.File> children = filesInFolder.Execute()
                        .Files;



                    List<string> fileNames = new List<string>();



                    Console.WriteLine("Files:");
                    if (children != null && children.Count > 0)
                    {
                        foreach (var child in children)
                        {
                            fileNames.Add(child.Name);
                            Console.WriteLine("{0} ({1})", child.Name, child.Id);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No files found.");
                    }
                    Console.Read();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("something went wrong");
                throw ex;
            }
            
        }

    }
}