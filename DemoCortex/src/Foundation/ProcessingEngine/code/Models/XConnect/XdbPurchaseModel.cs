using Sitecore.XConnect.Schema;

namespace Demo.Foundation.ProcessingEngine.Models.XConnect
{
    public static class XdbPurchaseModel
    {
        public static XdbModel Model { get; } = BuildModel();

        private static XdbModel BuildModel()
        {
            XdbModelBuilder modelBuilder = new XdbModelBuilder("PurchaseOutcome", new XdbModelVersion(1, 0));

            modelBuilder.ReferenceModel(Sitecore.XConnect.Collection.Model.CollectionModel.Model);
            modelBuilder.DefineEventType<PurchaseOutcome>(false);
            return modelBuilder.BuildModel();
        }
    }

   
}