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
using System.Text.RegularExpressions;
using System.Globalization;

namespace App1
{    
    public partial class MainPage : ContentPage
    {
        //static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        //static string ApplicationName = "Google Sheets API .NET Quickstart";
        //private SheetsService Service = null;
        //private ValueRange SheetsObj = null;
        private SheetsAPI SheetObject;

        public MainPage(Models.DriveAPI api)
        {

        }

        public MainPage(Models.SheetsAPI api)
        {
            InitializeComponent();

            // Initialize
            var init = api;
            SheetObject = api;

            AddButton.IsEnabled = false;

            CategoryPicker.ItemsSource = SheetObject.GetCategories();
            CategoryPicker.SelectedIndex = 0;

        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {
            var t = ReportedDescription.Text;

            var wt = ReportedDescription.Text.ToString();

            Expense newExpense = new Expense
            {
                Date = DateTime.Now,
                Amount = ReportedPrice.Text.ToString(),
                Description = ReportedDescription.Text == null ? "" : ReportedDescription.Text.ToString(),
                Category = CategoryPicker.SelectedItem.ToString()
            };

            SheetObject.UpdateRequest(newExpense);
        }

        private void ReportedPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Remove previous formatting, or the decimal check will fail including leading zeros
            string value = ReportedPrice.Text.Replace(",", "")
                .Replace("$", "").Replace(".", "").TrimStart('0');
            decimal ul;
            //Check we are indeed handling a number
            if (decimal.TryParse(value, out ul))
            {
                ul /= 100;
                //Unsub the event so we don't enter a loop
                ReportedPrice.TextChanged -= ReportedPrice_TextChanged;
                //Format the text as currency
                ReportedPrice.Text = string.Format(CultureInfo.CreateSpecificCulture("en-US"), "{0:C2}", ul);
                ReportedPrice.TextChanged += ReportedPrice_TextChanged;
                
            }

            AddButton.IsEnabled = TextisValid(ReportedPrice.Text);
            if (!AddButton.IsEnabled)
            {
                ReportedPrice.Text = "$0.00";
            }
        }

        private bool TextisValid(string text)
        {
            Regex money = new Regex(@"^\$(\d{1,3}(\,\d{3})*|(\d+))(\.\d{2})?$");
            return money.IsMatch(text);
        }

    }
} 