using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using App1.Models;

namespace App1
{
    
    public partial class MainPage : ContentPage
    {
        //static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "Google Sheets API .NET Quickstart";
        private SheetsService Service = null;
        private ValueRange SheetsObj = null;
        private SheetsAPI SheetObject;

        public MainPage(Models.SheetsAPI api)
        {
            InitializeComponent();

            // Initialize
            var init = api;
            SheetObject = api;
        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {
            SheetObject.UpdateRequest();
        }

    }
} 