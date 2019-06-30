using System;
using Sitecore.XConnect;

namespace Demo.Foundation.ProcessingEngine.Models
{
    public class PurchaseOutcome : Outcome
    {

        public int Quantity { get; set; }
        public int InvoiceId { get; set; }

        public int CustomerId { get; set; }

        public double UnitPrice { get; set; }

        public string ProductId { get; set; }

        public PurchaseOutcome(Guid definitionId, DateTime timestamp, string currencyCode, decimal monetaryValue, int invoiceId, int quantity, int customerId, string stockCode) : base(definitionId, timestamp, currencyCode, monetaryValue)
        {
            this.Quantity = quantity;
            this.CustomerId = customerId;
            this.InvoiceId = invoiceId;
            this.UnitPrice = (double)monetaryValue;
            this.ProductId = stockCode;
        }

        public static Guid PurchaseEventDefinitionId { get; } = new Guid("F841E7C8-A12D-4376-94CD-296A37EDABCC");
    }
}