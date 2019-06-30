using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Demo.Foundation.ProcessingEngine.Facets;
using Demo.Foundation.ProcessingEngine.Services;
using Demo.Foundation.ProcessingEngine.Train.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sitecore.Processing.Engine.Abstractions;
using Sitecore.Processing.Engine.ML;
using Sitecore.Processing.Engine.ML.Abstractions;
using Sitecore.Processing.Engine.Projection;
using Sitecore.Processing.Engine.Storage.Abstractions;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;

namespace Demo.Foundation.ProcessingEngine.Train.Workers
{
    
    public class RfmTrainingWorker : IDeferredWorker
    {
        private readonly IModel<Interaction> _model;
        private readonly RfmTrainingWorkerOptionsDictionary _options;
        private readonly ITableStore _tableStore;
        private readonly ILogger<RfmTrainingWorker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public RfmTrainingWorker(
            ITableStoreFactory tableStoreFactory,
            IServiceProvider provider,
            ILogger<RfmTrainingWorker> logger,
            AllowedModelsDictionary modelsDictionary,
            RfmTrainingWorkerOptionsDictionary options,
            IServiceProvider serviceProvider)
        {

            this._tableStore = tableStoreFactory.Create(options.SchemaName);
            this._options = options;
            this._logger = logger;
            this._model = modelsDictionary.CreateModel<Interaction>(provider, options.ModelType, options.ModelOptions);
            this._serviceProvider = serviceProvider;
        }

        public RfmTrainingWorker(
            ITableStoreFactory tableStoreFactory,
            IServiceProvider provider,
            ILogger<RfmTrainingWorker> logger,
            AllowedModelsDictionary modelsDictionary,
            IReadOnlyDictionary<string, string> options,
            IServiceProvider serviceProvider)
            : this(tableStoreFactory, provider, logger, modelsDictionary, RfmTrainingWorkerOptionsDictionary.Parse(options), serviceProvider)
        {
        }



        public async Task RunAsync(CancellationToken token)
        {
            _logger.LogInformation("RfmTrainingWorker.RunAsync");
            
            IReadOnlyList<string> tableNames = _options.TableNames;
            List<Task<TableStatistics>> tableStatisticsTasks = new List<Task<TableStatistics>>(tableNames.Count);
            foreach (string tableName in tableNames)
                tableStatisticsTasks.Add(this._tableStore.GetTableStatisticsAsync(tableName, token));
            TableStatistics[] tableStatisticsArray = await Task.WhenAll(tableStatisticsTasks).ConfigureAwait(false);
            List<TableDefinition> tableDefinitionList = new List<TableDefinition>(tableStatisticsTasks.Count);
            for (int index = 0; index < tableStatisticsTasks.Count; ++index)
            {
                TableStatistics result = tableStatisticsTasks[index].Result;
                if (result == null)
                    this._logger.LogWarning(string.Format("Statistics data for {0} table could not be retrieved. It will not participate in model training.", (object)tableNames[index]));
                else
                    tableDefinitionList.Add(result.Definition);
            }
            ModelStatistics modelStatistics = await _model.TrainAsync(_options.SchemaName, token, tableDefinitionList.ToArray()).ConfigureAwait(false);
        
            if (modelStatistics !=null)
                await UpdateRfmFacets(modelStatistics as RfmStatistics, token);
        }

        public async Task UpdateRfmFacets(RfmStatistics statistics, CancellationToken token)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                using (var xdbContext = scope.ServiceProvider.GetService<IXdbContext>())
                {
                    foreach (var identifier in statistics.Customers)
                    {
                        var reference = new IdentifiedContactReference(XConnectService.IdentificationSource, identifier.CustomerId.ToString());
                        var contact = await xdbContext.GetContactAsync(reference, new ContactExpandOptions(
                            PersonalInformation.DefaultFacetKey,
                            EmailAddressList.DefaultFacetKey,
                            ContactBehaviorProfile.DefaultFacetKey,
                            RfmContactFacet.DefaultFacetKey
                        ));
                        if (contact != null)
                        {
                            var rfmFacet = contact.GetFacet<RfmContactFacet>(RfmContactFacet.DefaultFacetKey) ?? new RfmContactFacet();
                            rfmFacet.R = identifier.R;
                            rfmFacet.F = identifier.F;
                            rfmFacet.M = identifier.M;
                            rfmFacet.Recency = identifier.Recency;
                            rfmFacet.Frequency = identifier.Frequency;
                            rfmFacet.Monetary = (double)identifier.Monetary;
                            xdbContext.SetFacet(contact, RfmContactFacet.DefaultFacetKey, rfmFacet);

                            _logger.LogInformation(string.Format("Update RFM info: customerId={0}, R={1}, F={2}, M={3}, Recency={4}, Frequency={5}, Monetary={6}",
                                identifier.CustomerId, rfmFacet.R, rfmFacet.F, rfmFacet.M, rfmFacet.Recency, rfmFacet.Frequency, rfmFacet.Monetary));

                            await xdbContext.SubmitAsync(token);
                        }
                    }

                  
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
        }
    }
}
