using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using App1.Services;
using Google.Apis.Drive.v3;

namespace App1.Models
{
    public class SheetsAPI
    {
        public SheetsService SheetsService { get; set; }
        public DriveService DriveService { get; set; }
        public ValueRange SheetsObject { get; set; }
        public UserCredential Credential { get; set; }
        public ClientSecrets Secrets { get; set; }
        public TokenResponse Token { get; set; }
        public string SpreadSheetID { get; set; }
        public string Range { get; set; }
        public int LatestRow { get; set; }
        static string[] Scopes = { SheetsService.Scope.Spreadsheets, SheetsService.Scope.Drive };

        public SheetsAPI()
        {

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                try
                {
                    Secrets = new ClientSecrets()
                    {
                        ClientId = Constants.ClientID,
                        ClientSecret = Constants.ClientSecret
                    };

                    Token = new TokenResponse { RefreshToken = Constants.RefreshToken };

                    Credential = new UserCredential(new GoogleAuthorizationCodeFlow(
                        new GoogleAuthorizationCodeFlow.Initializer
                        {
                            ClientSecrets = Secrets
                        }),
                        "user",
                        Token);


                    SheetsService = new SheetsService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = Credential,
                        ApplicationName = Constants.ApplicationName
                    });

                    DriveService = new DriveService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = Credential,
                        ApplicationName = Constants.ApplicationName
                    });

                    //=================================================================

                    

                    // Make the initial get request 
                    SheetsObject = SheetsService.Spreadsheets.Values.Get(Constants.SpreadsheetID, Constants.Range).Execute();
                    LatestRow = SheetsObject.Values.Count;

                }
                catch (Exception ex)
                {
                    // error
                    Console.WriteLine("something went wrong");
                    throw ex;
                }
            }
        }

        public List<string> GetFilesFromFolder()
        {

            FilesResource.ListRequest getFolder = DriveService.Files.List();
            getFolder.Q = "mimeType='application/vnd.google-apps.folder' and name='Expenses'";
            //listRequest.PageSize = 10;
            getFolder.Fields = "nextPageToken, files(id, name)";
            getFolder.Spaces = "drive";


            // get FolderID
            IList<Google.Apis.Drive.v3.Data.File> folder = getFolder.Execute()
                .Files;

            var folderID = folder[0].Id;

            FilesResource.ListRequest filesInFolder = DriveService.Files.List();
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
            //nsole.Read();

            return fileNames;
        }

        public List<string> GetCategories()
        {
            List<string> Categories = null;

            SheetsObject = SheetsService.Spreadsheets.Values.Get(Constants.SpreadsheetID, Constants.CategoriesRange).Execute();

            IList<IList<Object>> values = SheetsObject.Values;

            if (values != null && values.Count > 0)
            {
                Categories = new List<string>();

                foreach (var row in values)
                {
                    // Print columns A and E, which correspond to indices 0 and 4.
                    Categories.Add(row[0].ToString());
                }
            }

            return Categories;
        }

        public void UpdateRequest(Expense expense)
        {
            var oblist = new List<object>()
            {
                expense.Date.ToString("MM/dd/yyyy"),
                expense.Amount,
                expense.Description,
                expense.Category
            };

            ValueRange ValueRange = new ValueRange();
            ValueRange.Values = new List<IList<object>> { oblist };
            string UpdateRange = "Transactions!B" + ((LatestRow++) + 5).ToString() + ":E";

            var UpdateRequest = SheetsService.Spreadsheets.Values.Update(ValueRange, Constants.SpreadsheetID, UpdateRange);

            // might have to change this to USERENTERED
            UpdateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

            var result = UpdateRequest.Execute();

        }

    }
}
