using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Sitecore.Framework.Conditions;
using Sitecore.Processing.Common.Extensions;
using Sitecore.Processing.Engine.Abstractions;

namespace Demo.Foundation.ProcessingEngine.Train.Workers
{
    public class RfmTrainingWorkerOptionsDictionary : DeferredWorkerOptionsDictionary
    {
        public const string ModelTypeKey = "ModelType";
        public const string SchemaNameKey = "SchemaName";
        public const string TableNamesKey = "TableNames";
        private const string TrainingWorkerOpenGenericTypeFormat = "Demo.Foundation.ProcessingEngine.Train.Workers.RfmTrainingWorker, Demo.Foundation.ProcessingEngine";

        public RfmTrainingWorkerOptionsDictionary(
          string workerEntityTypeString,
          string modelTypeString,
          string schemaName,
          IReadOnlyList<string> tableNames,
          IReadOnlyDictionary<string, string> modelOptions)
          : this(CreateValidatedDictionaryWithWorkerType(workerEntityTypeString, modelTypeString, schemaName, tableNames, modelOptions))
        {
        }

        [JsonConstructor]
        protected RfmTrainingWorkerOptionsDictionary(IDictionary<string, string> dictionary)
          : base(dictionary)
        {
        }

        public string ModelType => this[nameof(ModelType)];

        public string SchemaName => this[nameof(SchemaName)];

        public IReadOnlyDictionary<string, string> ModelOptions => GetOptionsWithoutReservedKeys(this);

        public IReadOnlyList<string> TableNames => this.DeserializeJsonValue<IReadOnlyList<string>>(nameof(TableNames));

        public static RfmTrainingWorkerOptionsDictionary Parse(
          IReadOnlyDictionary<string, string> options)
        {
            Condition.Requires(options, nameof(options)).IsNotNull().IsNotEmpty();
            string emptyRequiredString1 = options.GetNonEmptyRequiredString("ModelType");
            IReadOnlyList<string> tableNames = JsonConvert.DeserializeObject<IReadOnlyList<string>>(options.GetNonEmptyRequiredString("TableNames"));
            string emptyRequiredString2 = options.GetNonEmptyRequiredString("WorkerType");
            IReadOnlyDictionary<string, string> withoutReservedKeys = GetOptionsWithoutReservedKeys(options);
            string emptyOptionalString = options.GetNonEmptyOptionalString("SchemaName", (string)null);
            IDictionary<string, string> validatedDictionary = CreateValidatedDictionary(emptyRequiredString1, emptyOptionalString, tableNames, withoutReservedKeys);
            validatedDictionary.Add("WorkerType", emptyRequiredString2);
            return new RfmTrainingWorkerOptionsDictionary(validatedDictionary);
        }

        private static IDictionary<string, string> CreateValidatedDictionaryWithWorkerType(
          string workerEntityTypeString,
          string modelType,
          string schemaName,
          IReadOnlyList<string> tableNames,
          IReadOnlyDictionary<string, string> modelOptions)
        {
            Condition.Requires(workerEntityTypeString, nameof(workerEntityTypeString)).IsNotNullOrWhiteSpace();
            IDictionary<string, string> validatedDictionary = CreateValidatedDictionary(modelType, schemaName, tableNames, modelOptions);
            validatedDictionary.Add("WorkerType", string.Format(CultureInfo.InvariantCulture, TrainingWorkerOpenGenericTypeFormat, workerEntityTypeString));
            return validatedDictionary;
        }

        private static IReadOnlyDictionary<string, string> GetOptionsWithoutReservedKeys(
          IReadOnlyDictionary<string, string> options)
        {
            return options.Except("ModelType", "TableNames", "WorkerType", "SchemaName");
        }

        private static IDictionary<string, string> CreateValidatedDictionary(
          string modelType,
          string schemaName,
          IReadOnlyList<string> tableNames,
          IReadOnlyDictionary<string, string> modelOptions)
        {
            Condition.Requires(modelType, nameof(modelType)).IsNotNullOrWhiteSpace();
            Condition.Requires(schemaName, nameof(schemaName)).IsNotLongerThan<string>(50);
            Condition.Requires(tableNames, nameof(tableNames)).IsNotNull().IsNotEmpty();
            Condition.Requires(modelOptions, nameof(modelOptions)).IsNotNull();
            modelOptions.EnsureNotContainsKeys("ModelType", "TableNames", "WorkerType", "SchemaName");
            foreach (string tableName in tableNames)
                Condition.Requires(tableName, nameof(tableNames)).IsNotNullOrWhiteSpace().IsNotLongerThan(192, string.Format("{0} parameter should not contain table names longer than {1} characters.", (object)nameof(tableNames), (object)192));
            string str = modelType.Truncate(50);
            return new Dictionary<string, string>(modelOptions.ToDictionary(x => x.Key, x => x.Value))
            {
                [ModelTypeKey] = modelType,
                [SchemaNameKey] = (string.IsNullOrWhiteSpace(schemaName) ? str : schemaName),
                [TableNamesKey] = tableNames.SerializeToJson()
            };
        }
    }
}
