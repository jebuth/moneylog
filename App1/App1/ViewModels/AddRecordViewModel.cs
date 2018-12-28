using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using App1.Services;
using App1.Models;
using App1.ViewModels;
using System.Text.RegularExpressions;

namespace App1
{
    class AddRecordViewModel : ViewModelBase
    {
        private SheetsAPI GoogleSheetsService { get; set; }

        // List of spreadsheets inside "Expense" folder
        public IList<SheetObj> AvailableSheets { get; set; }
        public IList<string> Categories { get; set; }

        // Add button enabled
        public bool AddButtonIsEnabled {get; set;}

        // Expense
        public Expense Expense { get; set; }

        // Expense props
        public string Amount { get; set; }
        public string Description { get; set; }


        // Active sheet
        SheetObj _SelectedSheet;
        public SheetObj SelectedSheet
        {
            get { return _SelectedSheet; }
            set
            {
                if(SelectedSheet != value)
                {
                    _SelectedSheet = value;
                    OnPropertyChanged();
                }
            }
        }

        string _SelectedCategory;
        public string SelectedCategory
        {
            get { return _SelectedCategory; }
            set
            {
                if(SelectedCategory != value)
                {
                    _SelectedCategory = value;
                    OnPropertyChanged();
                }
                    
            }
        }

        public AddRecordViewModel(SheetsAPI _GoogleSheetsService)
        {
            GoogleSheetsService = _GoogleSheetsService;

            AvailableSheets = GoogleSheetsService.AvailableSheets;
            Categories = GoogleSheetsService.GetCategories();

        }

        public void AddExpense()
        {

            Expense = new Expense();
            Expense.Date = DateTime.Now;
            Expense.Amount = this.Amount;
            Expense.Description = this.Description;
            Expense.Category = SelectedCategory;

            var stop = 0;

            GoogleSheetsService.UpdateRequest(Expense, SelectedSheet);
        }

        //public bool TextIsValid(string text)
        //{
        //    Regex money = new Regex(@"^\$(\d{1,3}(\,\d{3})*|(\d+))(\.\d{2})?$");
        //    return money.IsMatch(text);
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}