using System;
using System.Collections.Generic;
using System.Text;
using App1.Models;

namespace App1.Services
{
    public class ExpenseServices
    {
        public List<Expense> Expenses { get; set; }

        public ExpenseServices()
        {

        }

        //public List<Expense> GetExpenses()
        //{
        //    //var list = new List<Expense>
        //    //{
        //    //    new Expense(DateTime.Now, 1.11f, "one dollar 11 cents", "Pets")
        //    //};

        //    //return list;
        //}
    }
}
