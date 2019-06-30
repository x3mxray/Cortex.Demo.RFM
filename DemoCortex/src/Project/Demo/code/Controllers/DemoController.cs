using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Demo.Foundation.ProcessingEngine.Models;
using Demo.Foundation.ProcessingEngine.Services;
using Sitecore.Analytics;

namespace Demo.Project.Demo.Controllers
{
    public class CreateContactModel
    {
        public string Number { get; set; }
        public string Email { get; set; }
    }

    public class DemoController : Controller
    {
        // GET: Demo
        public ActionResult Index()
        {
            return View(new CreateContactModel());
        }
        public ActionResult Demo()
        {
            return View();
        }
        public ActionResult DemoVip()
        {
            return View();
        }

        public async Task<bool> CreateContact(CreateContactModel model)
        {
            var purchaseService = new XConnectService();
            var created = await purchaseService.CreateContact(XConnectService.IdentificationSource, model.Number, model.Email);
            return created;
        }

        public void EndAll(int clear = 0)
        {
            if (Tracker.Current != null)
            {
                Tracker.Current.EndVisit(clear != 0);
            }
            HttpContext.Session.Abandon();
        }

        public Guid IdentifyByEmail(string email)
        {
            Tracker.Current.Session.IdentifyAs(XConnectService.IdentificationSourceEmail, email);
            return Tracker.Current.Contact.ContactId;
        }

        public void RegisterTestPurchase(string email)
        {
            Tracker.Current.Session.IdentifyAs(XConnectService.IdentificationSourceEmail, email);
            var customer = Tracker.Current.Session.Contact.Identifiers.First(x => x.Source == "demo");

            var ev = Tracker.MarketingDefinitions.Outcomes[PurchaseOutcome.PurchaseEventDefinitionId];
            var outcomeData = new Sitecore.Analytics.Data.OutcomeData(ev, "USD", 112233);
            // Never saved to xConnect, must be converted into a property on a custom model
            outcomeData.CustomValues.Add("Quantity", "5"); 
            outcomeData.CustomValues.Add("CustomerId", customer.Identifier);
            outcomeData.CustomValues.Add("InvoiceId", "123");
            outcomeData.CustomValues.Add("StockCode", "1112233");

            Tracker.Current.CurrentPage.RegisterOutcome(outcomeData);
        }

        public bool Test()
        {
            return true;
        }
    }
}