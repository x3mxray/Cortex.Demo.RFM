using System;
using System.Collections.Generic;
using System.Linq;
using Demo.Foundation.ProcessingEngine.Mappers;
using Demo.Foundation.ProcessingEngine.Models;
using Demo.Foundation.ProcessingEngine.Predict.Models;
using Demo.Foundation.ProcessingEngine.Train.Models;
using Microsoft.Extensions.Configuration;
using RestSharp;
using Sitecore.Processing.Engine.ML.Abstractions;
using Sitecore.Processing.Engine.Projection;

namespace Demo.Foundation.ProcessingEngine.Services
{
    public class MLNetService : IMLNetService
    {

        private readonly string _trainUrl;
        private readonly string _predictUrl;
        private readonly string _mlServerUrl;

        public MLNetService(IConfiguration configuration)
        {
            _mlServerUrl = configuration.GetValue<string>("MLServerUrl");
            _trainUrl =  configuration.GetValue<string>("TrainUrl");
            _predictUrl = configuration.GetValue<string>("PredictUrl");
        }

        public ModelStatistics Train(IReadOnlyList<IDataRow> data)
        {
            var customersData = CustomerMapper.MapToCustomers(data);

            var rfmCalculateService = new RfmCalculateService();
            var calculatedScores = rfmCalculateService.CalculateRfmScores(customersData);

            var businessData = calculatedScores.Select(x => new Rfm
            {
                R = x.R,
                F = x.F,
                M = x.M
            }).ToList();

            var client = new RestClient(_mlServerUrl);
            var request = new RestRequest(_trainUrl, Method.POST);
            request.AddJsonBody(businessData);
            var response = client.Execute<bool>(request);
            var ok = response.Data;
            if (!ok)
            {
                throw new Exception("something is wrong with ML engine, check it");
            }

            return new RfmStatistics{ Customers = calculatedScores };
        }

        public IReadOnlyList<PredictionResult> Evaluate(IReadOnlyList<IDataRow> data)
        {
            var validContacts = data.Where(x => x.Enabled() && !string.IsNullOrEmpty(x.GetContactEmail())).ToList();
            var rfmList = validContacts.Select(x => x.MapToRfmFacet()).Select(rfm => new ClusteringData
            {
                R = rfm.R,
                F = rfm.F,
                M = rfm.M
            }).ToList();

            var client = new RestClient(_mlServerUrl);
            var request = new RestRequest(_predictUrl, Method.POST);
            request.AddJsonBody(rfmList);
            var response = client.Execute<List<int>>(request);

            var predictions = response.Data;
            return validContacts.Select((t, i) => new PredictionResult {Email = t.GetContactEmail(), Cluster = predictions[i]}).ToList();
        }
    }

    public interface IMLNetService
    {
        ModelStatistics Train(IReadOnlyList<IDataRow> data);
        IReadOnlyList<PredictionResult> Evaluate(IReadOnlyList<IDataRow> data);
    }
}
