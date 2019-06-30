using System;

namespace Demo.Foundation.ProcessingEngine.Models
{
    public class PurchaseInvoice
    {
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Currency { get; set; }
        public int Number { get; set; }
        public int ContactId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Country { get; set; }
        public string StockCode { get; set; }

        public string Description { get; set; }
    }

}