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

namespace App1.Models
{
    public class SheetsAPI
    {
        public SheetsService Service { get; set; }
        public ValueRange SheetsObject { get; set; }
        public UserCredential Credential { get; set; }
        public ClientSecrets Secrets { get; set; }
        public TokenResponse Token { get; set; }
        public string SpreadSheetID  { get; set; }
        public string Range { get; set; }
        public int LatestRow { get; set; }
        //public SpreadsheetsResource.ValuesResource.GetRequest Request { get; set; }
        static string[] Scopes = { SheetsService.Scope.Spreadsheets};

        public SheetsAPI()
        {
            try
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

                        Service = new SheetsService(new BaseClientService.Initializer()
                        {
                            HttpClientInitializer = Credential,
                            ApplicationName = Constants.ApplicationName
                        });

                        // Make the initial get request 
                        //Request = Service.Spreadsheets.Values.Get(Constants.SpreadsheetID, Constants.Range);
                        SheetsObject = Service.Spreadsheets.Values.Get(Constants.SpreadsheetID, Constants.Range).Execute();
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
            catch(Exception eeeeeeeeeeeeeeeeee)
            {

            }
        }

        public void UpdateRequest()
        {
            var oblist = new List<object>() { DateTime.Now.ToString("MM/dd/yyyy"), 99.99, "updated from iphone", "category" };

            ValueRange ValueRange = new ValueRange();
            ValueRange.Values = new List<IList<object>> { oblist };
            string UpdateRange = "Transactions!B" + ((LatestRow++) + 5).ToString() + ":E";

            var UpdateRequest = Service.Spreadsheets.Values.Update(ValueRange, Constants.SpreadsheetID, UpdateRange);

            // might have to change this to USERENTERED
            UpdateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

            var result = UpdateRequest.Execute();

        }

    }
}
