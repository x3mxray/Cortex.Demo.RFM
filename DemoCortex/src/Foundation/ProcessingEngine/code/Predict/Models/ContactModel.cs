using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo.Foundation.ProcessingEngine.Facets;
using Demo.Foundation.ProcessingEngine.Services;
using Sitecore.ContentTesting.ML.Workers;
using Sitecore.Processing.Engine.ML.Abstractions;
using Sitecore.Processing.Engine.Projection;
using Sitecore.Processing.Engine.Storage.Abstractions;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;

namespace Demo.Foundation.ProcessingEngine.Predict.Models
{
  
    public class ContactModel : BaseWorker, IModel<Contact>
    {
        private readonly IMLNetService _mlNetService;
        private readonly ITableStoreFactory _tableStoreFactory;

        public ContactModel(IReadOnlyDictionary<string, string> options, IMLNetService mlNetService, ITableStoreFactory tableStoreFactory) : base(tableStoreFactory)
        {
            _mlNetService = mlNetService;
            _tableStoreFactory = tableStoreFactory;

            Projection = Sitecore.Processing.Engine.Projection.Projection.Of<Contact>().CreateTabular(
                "ContactModel",
                cfg => cfg
                    .Key("ContactId", c => c.Id)
                    .Attribute("Enabled", c => c.GetFacet<RfmContactFacet>()==null ? 0 : 1)
                    .Attribute("R", c => c.GetFacet<RfmContactFacet>()==null ? 0 : c.GetFacet<RfmContactFacet>().R)
                    .Attribute("F", c => c.GetFacet<RfmContactFacet>() == null ? 0 : c.GetFacet<RfmContactFacet>().F)
                    .Attribute("M", c => c.GetFacet<RfmContactFacet>() == null ? 0 : c.GetFacet<RfmContactFacet>().M)
                    .Attribute("Recency", c => c.GetFacet<RfmContactFacet>() == null ? 0 : c.GetFacet<RfmContactFacet>().Recency)
                    .Attribute("Frequency", c => c.GetFacet<RfmContactFacet>() == null ? 0 : c.GetFacet<RfmContactFacet>().Frequency)
                    .Attribute("Monetary", c => c.GetFacet<RfmContactFacet>() == null ? 0 : c.GetFacet<RfmContactFacet>().Monetary)
                    .Attribute("Email", c => c.Emails()?.PreferredEmail?.SmtpAddress, nullable: true));
        }

        public Task<ModelStatistics> TrainAsync(string schemaName, CancellationToken cancellationToken, params TableDefinition[] tables)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<object>> EvaluateAsync(string schemaName, CancellationToken cancellationToken, params TableDefinition[] tables)
        {
            var tableStore = _tableStoreFactory.Create(schemaName);
            var data = await GetDataRowsAsync(tableStore, tables.First().Name, cancellationToken);
            return _mlNetService.Evaluate(data);
        }

        public IProjection<Contact> Projection { get; }



    }
}
