using System;

namespace Protecc.Models
{
    public class IncomeData
    {
        public decimal Lohn { get; set; }
        public decimal Nebenerwerb { get; set; }
        public decimal Sonstige { get; set; }
        public decimal FromValue { get; set; }
        public decimal ToValue { get; set; }
        public decimal Gesamtwert { get; set; }
    }
}