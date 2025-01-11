using System;

namespace Protecc.Models
{
    public class IncomeData
    {
        public decimal Salary { get; set; }
        public decimal Sideline { get; set; }
        public decimal Other { get; set; }
        public decimal FromValue { get; set; }
        public decimal ToValue { get; set; }
        public decimal TotalIncome { get; set; }
    }
}