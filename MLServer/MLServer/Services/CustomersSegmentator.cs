using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.DataView;
using Microsoft.ML;
using Microsoft.ML.Data;
using MLServer.Extensions;
using MLServer.Helpers;
using MLServer.Models;

namespace MLServer.Services
{
    public class CustomersSegmentator
    {
        private static MLContext _mlContext;
        private static string TrainedModelFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.model");
        private static string TrainedModelFileCountry => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data2.model");
        private IDataView _testingDataView;
        private static ITransformer _transformer;
        private static ITransformer _transformerCountry;
        private static List<ProductStats> _stats;
        private static List<CountryStats> _statsCountry;

        private static List<string> _countries;
        private int RfmMaxForTests = 3;

        private static List<ProductStats> Stats
        {
            get
            {
                return _stats;
            }
            set
            {
                _stats = value;
            }
        }
        private static List<CountryStats> StatsCountry
        {
            get
            {
                return _statsCountry;
            }
            set
            {
                _statsCountry = value;
            }
        }
        private static ITransformer TrainModel
        {
            get
            {
                if (_transformer != null) return _transformer;
                return LoadModel();
            }
            set
            {
                _transformer = value;
                SaveModel(_transformer);
            }
        }
        private static ITransformer TrainModelCountry
        {
            get
            {
                if (_transformerCountry != null) return _transformerCountry;
                return LoadModel(TrainedModelFileCountry);
            }
            set
            {
                _transformerCountry = value;
                SaveModel(_transformerCountry, TrainedModelFileCountry);
            }
        }
        public ClusteringMetrics Train(List<Rfm> list)
        {
            _mlContext = new MLContext(6, 1);
            var dataView = _mlContext.Data.LoadFromEnumerable(list);

            var trainingDataView = _mlContext.Clustering.TrainTestSplit(dataView);

            _testingDataView = trainingDataView.TestSet;

            string featuresColumnName = "Features";

            var pipeline = _mlContext.Transforms
                .Concatenate(featuresColumnName, "R", "M", "F")
                .Append(_mlContext.Clustering.Trainers.KMeans(featuresColumnName, clustersCount: 5));

            var model = pipeline.Fit(trainingDataView.TrainSet);
            var metrics = Evaluate(model);

            TrainModel = model;
            return metrics;
        }

        public ClusteringMetrics Evaluate(ITransformer model)
        {
            Console.WriteLine("=============== Evaluating Model with Test data===============");

            var predictions = model.Transform(_testingDataView);

            var metrics = _mlContext.Clustering.Evaluate(predictions, score: "Score", features: "Features");

            Console.WriteLine();
            Console.WriteLine("Model quality metrics evaluation");
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"AvgMinScore: {metrics.AvgMinScore:P2}");
            Console.WriteLine($"Dbi: {metrics.Dbi:P2}");
            Console.WriteLine($"Nmi: {metrics.Nmi:P2}");
            Console.WriteLine("=============== End of model evaluation ===============");

            TrainModel = model;

            // Run test cases to identify clusters
            var predictionFunction = TrainModel.CreatePredictionEngine<ClusteringData, ClusteringPrediction>(_mlContext);
            var tests = new List<TestCase>();
            for (var r = 1; r <= RfmMaxForTests; r++)
            {
                for (var f = 1; f <= RfmMaxForTests; f++)
                {
                    for (var m = 1; m <= RfmMaxForTests; m++)
                    {
                        var data = new ClusteringData
                        {
                            R = r,
                            M = f,
                            F = m
                        };
                        var prediction = predictionFunction.Predict(data);
                        tests.Add(new TestCase
                        {
                            Data = data,
                            Cluster = prediction.SelectedClusterId
                        });
                    }
                }
            }

            var fileService = new FileService();
            fileService.ExportToCsv(tests);


            return metrics;
        }

        public List<int> Predict(List<ClusteringData> records)
        {
            var list = new List<ClusteringPrediction>();
            var predictionFunction = TrainModel.CreatePredictionEngine<ClusteringData, ClusteringPrediction>(_mlContext);

            foreach (var record in records)
            {
                list.Add(predictionFunction.Predict(record));
            }
            return list.Select(x => (int)x.SelectedClusterId).ToList();
        }

        private static ITransformer LoadModel(string path=null)
        {
            ITransformer model;
            var fname = path == null ? TrainedModelFile : path;
            using (var stream = new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                model = _mlContext.Model.Load(stream);
            }

            return model;
        }

        private static void SaveModel(ITransformer model, string path = null)
        {
            var fname = path == null ? TrainedModelFile : path;
            using (var fileStream = new FileStream(fname, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                _mlContext.Model.Save(model, fileStream);
            }
        }


        public void TrainForecast(List<ProductStats> list)
        {
            Stats = list;
            _mlContext = new MLContext(1);
            var trainingDataView = _mlContext.Data.LoadFromEnumerable(list);

            var trainer = _mlContext.Regression.Trainers.FastTreeTweedie(labelColumnName: "Label", featureColumnName: "Features");
            var trainingPipeline = _mlContext.Transforms
                .Concatenate(outputColumnName: "NumFeatures", 
                nameof(ProductStats.Year),
                nameof(ProductStats.Month),
                nameof(ProductStats.Units),
                nameof(ProductStats.Avg),
                nameof(ProductStats.Count),
                //nameof(ProductStats.Max), nameof(ProductStats.Min),
                nameof(ProductStats.Prev))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "CatFeatures", inputColumnName: nameof(ProductStats.ProductId)))
                .Append(_mlContext.Transforms.Concatenate(outputColumnName: "Features", "NumFeatures", "CatFeatures"))
                .Append(_mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(ProductStats.Next)))
                .Append(trainer);

            var crossValidationResults = _mlContext.Regression.CrossValidate(data: trainingDataView, estimator: trainingPipeline, numFolds: 6, labelColumn: "Label");
            var model = trainingPipeline.Fit(trainingDataView);

            TrainModel = model;

            //TestForecastPrediction();
        }

        public static List<ProductStats> ProductHistory(string productId)
        {
            return Stats.Where(x => x.ProductId == productId).OrderBy(x => x.Year).ThenBy(x => x.Month).ToList();
        }
        public static float ProductForecast(ProductStats data)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<ProductStats, ProductUnitPrediction>(TrainModel);

            ProductUnitPrediction prediction = predictionEngine.Predict(data);
            return prediction.Score;
        }

        public void TrainForecastCountry(List<CountryStats> list)
        {
            StatsCountry = list;
            _countries = list.Select(x => x.Country).Distinct().ToList();

            _mlContext = new MLContext(1);
            var trainingDataView = _mlContext.Data.LoadFromEnumerable(list);

            var trainer = _mlContext.Regression.Trainers.FastTreeTweedie(labelColumnName: "Label", featureColumnName: "Features");
            var trainingPipeline = _mlContext.Transforms
                .Concatenate(outputColumnName: "NumFeatures",
                nameof(CountryStats.Year),
                nameof(CountryStats.Month),
                nameof(CountryStats.Units),
                nameof(CountryStats.Avg),
                nameof(CountryStats.Count),
                //nameof(ProductStats.Max), nameof(ProductStats.Min),
                nameof(CountryStats.Prev))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "CatFeatures", inputColumnName: nameof(CountryStats.Country)))
                .Append(_mlContext.Transforms.Concatenate(outputColumnName: "Features", "NumFeatures", "CatFeatures"))
                .Append(_mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(CountryStats.Next)))
                .Append(trainer);

            var crossValidationResults = _mlContext.Regression.CrossValidate(data: trainingDataView, estimator: trainingPipeline, numFolds: 6, labelColumn: "Label");
            var model = trainingPipeline.Fit(trainingDataView);

            TrainModelCountry = model;

            //TestForecastPrediction();
        }

        public static List<CountryStats> CountryHistory(string country)
        {
            return StatsCountry.Where(x => x.Country == country).OrderBy(x => x.Year).ThenBy(x => x.Month).ToList();
        }
        public static float CountryForecast(CountryStats data)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<CountryStats, ProductUnitPrediction>(TrainModelCountry);

            ProductUnitPrediction prediction = predictionEngine.Predict(data);
            return prediction.Score;
        }

        public static List<string> GetCountries()
        {
            return _countries;
        }
    }
}
