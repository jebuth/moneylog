using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;

namespace App1
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LandingPage : ContentPage
	{
        private ValueRange CurrentSheet;
        private int RowToInsert;

        private SheetsService Service;

        public LandingPage(ValueRange SheetObj, SheetsService Service)
        {
            InitializeComponent();
            CurrentSheet = SheetObj;
            RowToInsert = CurrentSheet.Values.Count;
            this.Service = Service;
        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {

            var stop = 0;
            IList<IList<Object>> values = CurrentSheet.Values;
            if (values != null && values.Count > 0)
            {
                Console.WriteLine("Name, Major");
                foreach (var row in values)
                {
                    // Print columns A and E, which correspond to indices 0 and 4.
                    Console.WriteLine("{0}, {1}", row[0], row[1]);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }

            string price = ReportedPrice.Text.ToString();
            string cat = SelectedCategory.Text.ToString();

            var oblist = new List<object>() { DateTime.Now.ToString("MM/dd/yyyy"), price, "updated from iphone", cat };

            ValueRange valueRange = new ValueRange();
            valueRange.Values = new List<IList<object>> { oblist };


            String spreadsheetId = "15lIK4Iox1H-E20mwtRBhJkhNCAswQlCtuDxgf8NUcvc";
            string updateRange = "Transactions!B" + (RowToInsert++ + 5).ToString() + ":E";

            SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest =
                Service.Spreadsheets.Values.Update(valueRange, spreadsheetId, updateRange);

            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            var result = updateRequest.Execute();

        }
    }
}