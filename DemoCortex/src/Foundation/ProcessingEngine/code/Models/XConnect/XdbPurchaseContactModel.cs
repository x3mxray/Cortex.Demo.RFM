using Demo.Foundation.ProcessingEngine.Facets;
using Sitecore.XConnect;
using Sitecore.XConnect.Schema;

namespace Demo.Foundation.ProcessingEngine.Models.XConnect
{
    public static class XdbPurchaseContactModel
    {
        public static XdbModel Model { get; } = BuildModel();

        private static XdbModel BuildModel()
        {
            XdbModelBuilder modelBuilder = new XdbModelBuilder("ContactModel", new XdbModelVersion(1, 0));

            modelBuilder.ReferenceModel(Sitecore.XConnect.Collection.Model.CollectionModel.Model);
            modelBuilder.DefineFacet<Contact, RfmContactFacet>(RfmContactFacet.DefaultFacetKey);

            return modelBuilder.BuildModel();
        }
    }
}
