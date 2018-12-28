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
using App1.Services;
using System.Text.RegularExpressions;
using System.Globalization;

namespace App1
{    
    public partial class AddRecord : ContentPage
    {
        //private SheetsAPI GoogleSheetsService { get; set; }
        private SheetObj ActiveSheet{ get; set; }
        //private AddRecordViewModel ViewModel { get; set; }

        public AddRecord()
        {
           // BindingContext = new AddRecordViewModel();

            //InitializeComponent();
        }


        public AddRecord(SheetsAPI _GoogleSheetsService)
        {

            BindingContext = new AddRecordViewModel(_GoogleSheetsService);

            InitializeComponent();

            //try
            //{
            //    if (GoogleSheetsService == null)
            //    {
            //        GoogleSheetsService = _GoogleSheetsService;
            //        //ViewModel = new AddRecordViewModel();
            //        BindingContext = new AddRecordViewModel(_GoogleSheetsService);     
                    
            //    }
            //}

            //catch(Exception ex)
            //{

            //}  
        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {
            (BindingContext as AddRecordViewModel).AddExpense();
        }

        private void ReportedPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Remove previous formatting, or the decimal check will fail including leading zeros
            string value = ReportedPrice.Text.Replace(",", "")
                .Replace("$", "").Replace(".", "").TrimStart('0');
            //Check we are indeed handling a number
            if (decimal.TryParse(value, out decimal ul))
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

        private void SheetPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var SelectedSpreadsheet = this.SheetPicker.SelectedItem as SheetObj;
            //var s = this.SheetPicker.SelectedItem.ToString();

            //this.ActiveSheet = this.SheetPicker.SelectedItem as SheetObj;

            //var who = SelectedSpreadsheet.Title;

            //var sheetID = SheetObject.GetIDByTitle(s);

            //SheetObject.UpdateActiveSheet(sheetID);


            // the drop down should already be populated by SheetObjs so 
            // we don't need to do all this bs


        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {

        }
    }
} 