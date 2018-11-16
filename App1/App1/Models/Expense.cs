using System;

namespace App1.Models
{
    public class Expense
    {
        public DateTime Date { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

        //public Expense(DateTime date, float amount, string description, string category)
        //{
        //    Date = date;
        //    Amount = amount;
        //    Description = description;
        //    Category = category;

        //}

    }
}
