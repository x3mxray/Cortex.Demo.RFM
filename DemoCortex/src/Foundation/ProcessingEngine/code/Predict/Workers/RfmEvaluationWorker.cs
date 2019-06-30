using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo.Foundation.ProcessingEngine.Facets;
using Demo.Foundation.ProcessingEngine.Mappers;
using Demo.Foundation.ProcessingEngine.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sitecore.Processing.Engine.ML.Abstractions;
using Sitecore.Processing.Engine.ML.Workers;
using Sitecore.Processing.Tasks.Options.Workers.ML;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;

namespace Demo.Foundation.ProcessingEngine.Predict.Workers
{
    public class RfmEvaluationWorker : EvaluationWorker<Contact>
    {
        private readonly ILogger<RfmEvaluationWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        public RfmEvaluationWorker(IModelEvaluator evaluator, IReadOnlyDictionary<string, string> options, ILogger<RfmEvaluationWorker> logger, IServiceProvider serviceProvider) : base(evaluator, options)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public RfmEvaluationWorker(IModelEvaluator evaluator, EvaluationWorkerOptionsDictionary options, ILogger<RfmEvaluationWorker> logger, IServiceProvider serviceProvider) : base(evaluator, options)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }


        // Update Cluster for Contact
        protected override async Task ConsumeEvaluationResultsAsync(IReadOnlyList<Contact> entities, IReadOnlyList<object> evaluationResults, CancellationToken token)
        {
            var contactIdentifiers = entities
                .SelectMany(x =>
                    x.Identifiers.Where(s => s.Source == XConnectService.IdentificationSourceEmail));

            var predictionResults = evaluationResults.ToPredictionResults();

            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                using (var xdbContext = scope.ServiceProvider.GetRequiredService<IXdbContext>())
                {
                    foreach (var identifier in contactIdentifiers)
                    {
                        // filter unknown contacts (with Email=null)
                        if (predictionResults.Any(x => x.Email.Equals(identifier.Identifier)))
                        {
                            var reference = new IdentifiedContactReference(identifier.Source, identifier.Identifier);
                            var contact = await xdbContext.GetContactAsync(reference, new ContactExpandOptions(
                                PersonalInformation.DefaultFacetKey,
                                EmailAddressList.DefaultFacetKey,
                                ContactBehaviorProfile.DefaultFacetKey,
                                RfmContactFacet.DefaultFacetKey
                            ));
                            if (contact != null)
                            {
                                var rfmFacet = contact.GetFacet<RfmContactFacet>(RfmContactFacet.DefaultFacetKey) ?? new RfmContactFacet();
                                rfmFacet.Cluster = predictionResults.First(x => x.Email.Equals(identifier.Identifier)).Cluster;
                                xdbContext.SetFacet(contact, RfmContactFacet.DefaultFacetKey, rfmFacet);

                                _logger.LogInformation(string.Format("RFM info: email={0}, R={1}, F={2}, M={3}, Recency={4}, Frequency={5}, Monetary={6}, CLUSTER={7}",
                                    identifier.Identifier, rfmFacet.R, rfmFacet.F, rfmFacet.M, rfmFacet.Recency, rfmFacet.Frequency, rfmFacet.Monetary, rfmFacet.Cluster));

                            }
                        }
                       
                    }

                    await xdbContext.SubmitAsync(token);
                }
            }
        }
    }
}
