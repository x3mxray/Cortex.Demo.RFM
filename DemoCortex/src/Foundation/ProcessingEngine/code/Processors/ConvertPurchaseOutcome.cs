using Demo.Foundation.ProcessingEngine.Models;
using Sitecore.Analytics.Model;
using Sitecore.Analytics.XConnect.DataAccess.Pipelines.ConvertToXConnectEventPipeline;
using Sitecore.XConnect;

namespace Demo.Foundation.ProcessingEngine.Processors
{

    public class ConvertPurchaseOutcome : ConvertToXConnectEventProcessorBase<OutcomeData>
    {
        protected override Event ConvertToEvent(OutcomeData entity)
        {
            var quantity = int.Parse(entity.CustomValues["Quantity"] as string);
            var invoiceId = int.Parse(entity.CustomValues["InvoiceId"] as string);
            var contactId = int.Parse(entity.CustomValues["CustomerId"] as string);
            var stockCode = entity.CustomValues["StockCode"] as string;

            var purchase = new PurchaseOutcome(PurchaseOutcome.PurchaseEventDefinitionId, entity.Timestamp,  entity.CurrencyCode, entity.MonetaryValue,invoiceId,quantity, contactId, stockCode);

            return purchase;
        }

        protected override bool CanProcess(Sitecore.Analytics.Model.Entity entity)
        {
            if (entity is OutcomeData outcomeData)
            {
                return (outcomeData.OutcomeDefinitionId == PurchaseOutcome.PurchaseEventDefinitionId);
            }

            return false;
        }
    }
}