using System.Collections.Generic;
using System.Linq;
using Demo.Foundation.ProcessingEngine.Models;
using Sitecore.Processing.Engine.Projection;

namespace Demo.Foundation.ProcessingEngine.Mappers
{
    public static class CustomerMapper
    {

        public static List<PurchaseInvoice> MapToProductsList(IReadOnlyList<IDataRow> dataRows)
        {
            var invoices = new List<PurchaseInvoice>();
            foreach (var data in dataRows)
            {
                invoices.Add(data.ToPurchaseOutcome());
            }
            return invoices;
        }

        public static List<Customer> MapToCustomers(IReadOnlyList<IDataRow> dataRows)
        {
            var invoices = new List<PurchaseInvoice>();
            foreach (var data in dataRows)
            {
                invoices.Add(data.ToPurchaseOutcome());
            }

            var groupedData = invoices.AsEnumerable().GroupBy(x => x.ContactId);

            return groupedData.Select(data => new Customer {CustomerId = data.Key, Invoices = data.ToList()}).ToList();
        }

        public static PurchaseInvoice ToPurchaseOutcome(this IDataRow dataRow)
        {
            var result = new PurchaseInvoice();
            var customerId = dataRow.Schema.Fields.FirstOrDefault(x => x.Name == nameof(PurchaseOutcome.CustomerId));
            if (customerId != null)
            {
                result.ContactId = (int) dataRow.GetInt64(dataRow.Schema.GetFieldIndex(nameof(PurchaseOutcome.CustomerId)));
            }
            var invoiceId = dataRow.Schema.Fields.FirstOrDefault(x => x.Name == nameof(PurchaseOutcome.InvoiceId));
            if (invoiceId != null)
            {
                result.Number = (int)dataRow.GetInt64(dataRow.Schema.GetFieldIndex(nameof(PurchaseOutcome.InvoiceId)));
            }

            var quantity = dataRow.Schema.Fields.FirstOrDefault(x => x.Name == nameof(PurchaseOutcome.Quantity));
            if (quantity != null)
            {
                result.Quantity = (int)dataRow.GetInt64(dataRow.Schema.GetFieldIndex(nameof(PurchaseOutcome.Quantity)));
            }

            var date = dataRow.Schema.Fields.FirstOrDefault(x => x.Name == nameof(PurchaseOutcome.Timestamp));
            if (date != null)
            {
                result.TimeStamp = dataRow.GetDateTime(dataRow.Schema.GetFieldIndex(nameof(PurchaseOutcome.Timestamp)));
            }

            var price = dataRow.Schema.Fields.FirstOrDefault(x => x.Name == nameof(PurchaseOutcome.UnitPrice));
            if (price != null)
            {
                result.Price = (decimal) dataRow.GetDouble(dataRow.Schema.GetFieldIndex(nameof(PurchaseOutcome.UnitPrice)));
            }

            var productId = dataRow.Schema.Fields.FirstOrDefault(x => x.Name == nameof(PurchaseOutcome.ProductId));
            if (productId != null)
            {
                result.StockCode = dataRow.GetString(dataRow.Schema.GetFieldIndex(nameof(PurchaseOutcome.ProductId)));
            }

            var country = dataRow.Schema.Fields.FirstOrDefault(x => x.Name == "Country");
            if (country != null)
            {
                result.Country = dataRow.GetString(dataRow.Schema.GetFieldIndex("Country"));
            }

            return result;
        }
     
    }
}
