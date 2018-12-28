using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using App1.Models;
using Google.Apis.Drive.v3;


namespace App1.Services
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

        //important!
        //public SheetObj ActiveSheet{ get; set; }

        // list of available sheets
        public List<SheetObj> AvailableSheets = null;
        public List<string> AvailableSheetTitles = null;


        public SheetsAPI()
        {
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                try
                {
                    Secrets = new ClientSecrets()
                    {
                        ClientId = GoogleAccess.ClientID,
                        ClientSecret = GoogleAccess.ClientSecret
                    };

                    Token = new TokenResponse { RefreshToken = GoogleAccess.RefreshToken, AccessToken= GoogleAccess.AccessToken };

                    Credential = new UserCredential(new OfflineAccessGoogleAuthorizationCodeFlow(
                        new GoogleAuthorizationCodeFlow.Initializer
                        {
                            ClientSecrets = Secrets
                        }),
                        "user",
                        Token);
                    
                   // Constants.AccessToken = Credential.Token.AccessToken;

                    SheetsService = new SheetsService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = Credential,
                        ApplicationName = GoogleAccess.ApplicationName
                    });

                    DriveService = new DriveService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = Credential,
                        ApplicationName = GoogleAccess.ApplicationName
                    });

                    //=================================================================
                    
                    // Make the initial get request 
                    SheetsObject = SheetsService.Spreadsheets.Values.Get(GoogleAccess.SpreadsheetID, Constants.Range).Execute();
                    LatestRow = SheetsObject.Values.Count;

                    GetFilesFromFolder();

                }
                catch (Exception ex)
                {
                    // error
                    Console.WriteLine("something went wrong");
                    throw ex;
                }
            }
        }

        public List<SheetObj> GetFilesFromFolder()
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
            //filesInFolder.Fields = "nextPageToken, files(id, name)";
            filesInFolder.Fields = @"files(*)";
            filesInFolder.Spaces = "drive";

            IList<Google.Apis.Drive.v3.Data.File> children = filesInFolder.Execute()
                .Files;
            
            List<string> fileNames = new List<string>();
            AvailableSheets = new List<SheetObj>();
            AvailableSheetTitles = new List<string>();


            Console.WriteLine("Files:");
            if (children != null && children.Count > 0)
            {
                foreach (var child in children)
                {
                    //fileNames.Add(child.Name);
                    AvailableSheets.Add(new SheetObj { ID = child.Id, Title = child.Name, LastModified = child.ModifiedTime });


                    // don't need this????
                    AvailableSheetTitles.Add(child.Name);

                    // Console.WriteLine("{0} ({1})", child.Name, child.Id);
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }

            return AvailableSheets;
        }

        public List<string> GetCategories()
        {
            List<string> Categories = null;

            SheetsObject = SheetsService.Spreadsheets.Values.Get(GoogleAccess.SpreadsheetID, Constants.CategoriesRange).Execute();

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

        public void UpdateRequest(Expense expense, SheetObj ActiveSheet)
        {
            var oblist = new List<object>()
            {
                expense.Date.ToString("MM/dd/yyyy"),
                expense.Amount,
                expense.Description,
                expense.Category
            };

            // check ActiveSheet's LatestRow

            if(LatestRow != ActiveSheet.LatestRow)
            {
                SheetsObject = SheetsService.Spreadsheets.Values.Get(ActiveSheet.ID, Constants.Range).Execute();
                ActiveSheet.LatestRow = SheetsObject.Values.Count;
                LatestRow = ActiveSheet.LatestRow;
            }


            ValueRange ValueRange = new ValueRange();
            ValueRange.Values = new List<IList<object>> { oblist };
            //string UpdateRange = "Transactions!B" + ((LatestRow++) + 5).ToString() + ":E";
            ActiveSheet.UpdateRange = "Transactions!B" + ((ActiveSheet.LatestRow++) + 5).ToString() + ":E";

            //var UpdateRequest = SheetsService.Spreadsheets.Values.Update(ValueRange, GoogleAccess.SpreadsheetID, UpdateRange);
            var UpdateRequest = SheetsService.Spreadsheets.Values.Update(ValueRange, ActiveSheet.ID, ActiveSheet.UpdateRange);

            // might have to change this to USERENTERED
            UpdateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            var result = UpdateRequest.Execute();

        }

        public List<string> GetAvailableTitles()
        {
            return AvailableSheetTitles;
        }



    }
}
