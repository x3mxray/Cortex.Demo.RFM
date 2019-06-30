using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.DataView;
using Microsoft.ML;
using Microsoft.ML.Data;
using MLServer.Helpers;
using MLServer.Models;

namespace MLServer.Services
{
    public class CustomersSegmentator
    {
        private static MLContext _mlContext;
        private static string TrainedModelFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.model");
        private IDataView _testingDataView;
        private static ITransformer _transformer;

        private int RfmMaxForTests = 3;

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
            var predictions = model.Transform(_testingDataView);

            var metrics = _mlContext.Clustering.Evaluate(predictions, score: "Score", features: "Features");

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

            // save RFM cluster matching in csv
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

    }
}
