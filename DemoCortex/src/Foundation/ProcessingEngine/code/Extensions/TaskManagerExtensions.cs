using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Foundation.ProcessingEngine.Facets;
using Demo.Foundation.ProcessingEngine.Predict.Models;
using Demo.Foundation.ProcessingEngine.Predict.Workers;
using Demo.Foundation.ProcessingEngine.Train.Models;
using Demo.Foundation.ProcessingEngine.Train.Workers;
using Sitecore.Processing.Engine.Abstractions;
using Sitecore.Processing.Tasks.Options;
using Sitecore.Processing.Tasks.Options.DataSources.DataExtraction;
using Sitecore.Processing.Tasks.Options.Workers.ML;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;

namespace Demo.Foundation.ProcessingEngine.Extensions
{
    public static class TaskManagerExtensionsCustom
    {
        public static async Task RegisterRfmModelTaskChainAsync(
          this ITaskManager taskManager,
          TimeSpan expiresAfter)
        {
            // Define workers parameters

            // datasource for PurchaseOutcomeModel projection
            var interactionDataSourceOptionsDictionary = new InteractionDataSourceOptionsDictionary(new InteractionExpandOptions(IpInfo.DefaultFacetKey), 5, 10);
            // datasource for ContactModel protection 
            var contactDataSourceOptionsDictionary = new ContactDataSourceOptionsDictionary(new ContactExpandOptions(PersonalInformation.DefaultFacetKey,
                    EmailAddressList.DefaultFacetKey,
                    ContactBehaviorProfile.DefaultFacetKey,
                    RfmContactFacet.DefaultFacetKey)
                , 5, 10);

            var modelTrainingOptions = new ModelTrainingTaskOptions(
                // assembly name of our processing engine model (PurchaseInteractionModel:IModel<Interaction>) 
                typeof(PurchaseInteractionModel).AssemblyQualifiedName,
                // assembly name of entity for our processing engine model  (PurchaseInteractionModel:IModel<Interaction>) 
                typeof(Interaction).AssemblyQualifiedName,
                // custom options that we pass to PurchaseInteractionModel
                new Dictionary<string, string> { ["TestCaseId"] = "Id" },
                // projection tableName of PurchaseOutcomeModel, must be equal to first parameter of 'CreateTabular' method => PurchaseOutcomeModel.cs: CreateTabular("PurchaseOutcome", ...)
                "PurchaseOutcome",
                // name of resulted table (any name)
                "DemoResultTable");

            var projectionDictionary = new ProjectionWorkerOptionsDictionary(
                modelTrainingOptions.ModelEntityTypeString,
                modelTrainingOptions.ModelTypeString, expiresAfter, modelTrainingOptions.SchemaName,
                modelTrainingOptions.ModelOptions);


            var evaluationDictionary = new EvaluationWorkerOptionsDictionary(
                 typeof(RfmEvaluationWorker).AssemblyQualifiedName,
                typeof(ContactModel).AssemblyQualifiedName,
                new Dictionary<string, string> { ["TestCaseId"] = "Id" },
                "Evaluator.Schema",
                expiresAfter);


            // Register chain of Tasks

            // 1) Register Projection-worker
            Guid projectionTaskId = await taskManager.RegisterDistributedTaskAsync(
                interactionDataSourceOptionsDictionary,
                projectionDictionary,
                // no prerequisite tasks
                Enumerable.Empty<Guid>(),
                expiresAfter).ConfigureAwait(false);

            // 2) Register Merge-worker
            var mergeTaskIds = new List<Task<Guid>>();
            foreach (var targetTableNames in modelTrainingOptions.SourceTargetTableNamesMap)
            {
                var mergeWorkerOptionsDictionary = new MergeWorkerOptionsDictionary(targetTableNames.Value, targetTableNames.Key, expiresAfter, modelTrainingOptions.SchemaName);
                mergeTaskIds.Add(
                    taskManager.RegisterDeferredTaskAsync(
                        mergeWorkerOptionsDictionary,
                        // execute after Projection task
                        new[] { projectionTaskId },
                        expiresAfter));
            }
            await Task.WhenAll(mergeTaskIds).ConfigureAwait(false);


            // 3) Register Train-worker
            var trainWorkerOptionsDictionary = new RfmTrainingWorkerOptionsDictionary(
                modelTrainingOptions.ModelEntityTypeString,
                modelTrainingOptions.ModelTypeString,
                modelTrainingOptions.SchemaName,
                modelTrainingOptions.SourceTargetTableNamesMap.Values.ToList(),
                modelTrainingOptions.ModelOptions);

            Guid trainTaskId = await taskManager.RegisterDeferredTaskAsync(
                trainWorkerOptionsDictionary,
                // execute after Merge task
                mergeTaskIds.Select(t => t.Result),
                expiresAfter).ConfigureAwait(false);

            // 4) Register Evaluate worker
            Guid evaluateTaskId = await taskManager.RegisterDistributedTaskAsync(
                    contactDataSourceOptionsDictionary,
                    evaluationDictionary,
                    // execute after Train worker
                    new[] { trainTaskId },
                    expiresAfter)
                .ConfigureAwait(false);
        }
    }
}
