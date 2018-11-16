using System;

namespace App1.Models
{
    public class Expense
    {
        public DateTime Date { get; set; }
        private float Amount { get; set; }
        private string Description { get; set; }
        private string Category { get; set; }

        public Expense(DateTime date, float amount, string description, string category)
        {
            Date = date;
            Amount = amount;
            Description = description;
            Category = category;

        }

    }
}
