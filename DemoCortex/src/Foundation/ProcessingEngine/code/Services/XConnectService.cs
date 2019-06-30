using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Foundation.ProcessingEngine.Models;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Client.Configuration;
using Sitecore.XConnect.Collection.Model;

namespace Demo.Foundation.ProcessingEngine.Services
{
    public class XConnectService
    {
        public static string IdentificationSource => "demo";
        public static string IdentificationSourceEmail => "demo_email";

        static readonly ID CountryFolder = new ID("{DBE138C0-160F-4540-9868-0098E2CE8174}");
        private static List<Item> _countries;

        public string GetCountryCodeByName(string name)
        {
            if (_countries == null)
            {
                var folder = Sitecore.Context.Database.GetItem(CountryFolder);
                if (folder != null)
                {
                    _countries = folder.Children.ToList();
                }
            }

            var country = _countries?.FirstOrDefault(x => x.Name == name);
            if (country != null)
            {
                return country["Country Code"];
            }
            return name;
        }

        public async Task<bool> CreateContact(
            string source,
            string identifier,
            string email)
        {
            using (XConnectClient client = SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {
                    IdentifiedContactReference reference = new IdentifiedContactReference(source, identifier);

                    var contactTask = client.GetAsync(
                        reference,
                        new ContactExpandOptions()
                    );

                    Contact existingContact = await contactTask;

                    if (existingContact != null)
                    {
                        return false;
                    }


                    Contact contact = new Contact(new ContactIdentifier(source, identifier, ContactIdentifierType.Known));

                    var preferredEmail = new EmailAddress(email, true);
                    var emails = new EmailAddressList(preferredEmail, "Work");

                    client.AddContact(contact);
                    client.SetEmails(contact, emails);

                    var identifierEmail = new ContactIdentifier(IdentificationSourceEmail, email, ContactIdentifierType.Known);

                    client.AddContactIdentifier(contact, identifierEmail);

                    await client.SubmitAsync();

                    return true;
                }
                catch (XdbExecutionException ex)
                {
                    Log.Error(ex.Message, ex, this);
                    return false;
                }
            }
        }

        // if addWebVisit=true, fake webvisit will be created for interaction 
        // it is needed if you want to populate interaction country (to use contacts country for ML data model)
        public async Task<bool> Add(Customer purchase, bool addWebVisit = false)
        {
            using (XConnectClient client = SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {

                    IdentifiedContactReference reference = new IdentifiedContactReference(IdentificationSource, purchase.CustomerId.ToString());
                    var customer = await client.GetAsync(
                        reference,
                        new ContactExpandOptions(
                            PersonalInformation.DefaultFacetKey,
                            EmailAddressList.DefaultFacetKey,
                            ContactBehaviorProfile.DefaultFacetKey
                        )
                        {
                            Interactions = new RelatedInteractionsExpandOptions
                            {
                                StartDateTime = DateTime.MinValue,
                                EndDateTime = DateTime.MaxValue,
                                Limit = 100
                            }
                        }
                    );

                    if (customer == null)
                    {
                        var email = "demo" + Guid.NewGuid().ToString("N") + "@gmail.com";

                        customer = new Contact(new ContactIdentifier(IdentificationSource, purchase.CustomerId.ToString(), ContactIdentifierType.Known));

                        var preferredEmail = new EmailAddress(email, true);
                        var emails = new EmailAddressList(preferredEmail, "Work");

                        client.AddContact(customer);
                        client.SetEmails(customer, emails);

                        var identifierEmail = new ContactIdentifier(IdentificationSourceEmail, email, ContactIdentifierType.Known);
                        
                        client.AddContactIdentifier(customer, identifierEmail);
                        client.Submit();
                    }

                    var channel = Guid.Parse("DF9900DE-61DD-47BF-9628-058E78EF05C6");
                    var interaction = new Interaction(customer, InteractionInitiator.Brand, channel, "demo app");

                    if (addWebVisit)
                    {
                        //Add Device profile
                        DeviceProfile newDeviceProfile = new DeviceProfile(Guid.NewGuid()) { LastKnownContact = customer };
                        client.AddDeviceProfile(newDeviceProfile);
                        interaction.DeviceProfile = newDeviceProfile;

                        //Add fake Ip info
                        IpInfo fakeIpInfo = new IpInfo("127.0.0.1") { BusinessName = "Home"};
                        var country = purchase.Invoices.FirstOrDefault(x => !string.IsNullOrEmpty(x.Country))?.Country;
                        fakeIpInfo.Country = GetCountryCodeByName(country);

                        client.SetFacet(interaction, IpInfo.DefaultFacetKey, fakeIpInfo);

                        // Add fake webvisit
                        // Create a new web visit facet model
                        var webVisitFacet = new WebVisit
                        {
                            Browser =
                                new BrowserData
                                {
                                    BrowserMajorName = "Chrome",
                                    BrowserMinorName = "Desktop",
                                    BrowserVersion = "22.0"
                                },
                            Language = "en",
                            OperatingSystem =
                                new OperatingSystemData { Name = "Windows", MajorVersion = "10", MinorVersion = "4" },
                            Referrer = "https://www.google.com",
                            Screen = new ScreenData { ScreenHeight = 1080, ScreenWidth = 685 },
                            SearchKeywords = "sitecore",
                            SiteName = "website"
                        };

                        // Populate data about the web visit

                        var itemId = Guid.Parse("110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9");
                        var itemVersion = 1;

                        // First page view
                        var datetime = purchase.Invoices.FirstOrDefault() == null
                            ? DateTime.Now
                            : purchase.Invoices.First().TimeStamp.ToUniversalTime();
                        PageViewEvent pageView = new PageViewEvent(datetime,
                            itemId, itemVersion, "en")
                        {
                            ItemLanguage = "en",
                            Duration = new TimeSpan(3000),
                            Url = "/home"
                        };
                        // client.SetFacet(interaction, WebVisit.DefaultFacetKey, webVisitFacet);
                        
                        interaction.Events.Add(pageView);
                        client.SetWebVisit(interaction, webVisitFacet);
                    }



                    foreach (var invoice in purchase.Invoices)
                    {
                        var outcome = new PurchaseOutcome(PurchaseOutcome.PurchaseEventDefinitionId, invoice.TimeStamp, invoice.Currency, invoice.Price, invoice.Number, invoice.Quantity, purchase.CustomerId, invoice.StockCode);
                        interaction.Events.Add(outcome);
                    }

                    client.AddInteraction(interaction);

                    await client.SubmitAsync();

                    return true;
                }
                catch (XdbExecutionException ex)
                {
                    Log.Error(ex.Message, ex, this);
                    return false;
                }
            }
        }
    }
}