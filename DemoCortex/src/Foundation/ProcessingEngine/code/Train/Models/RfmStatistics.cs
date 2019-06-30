using System.Collections.Generic;
using Demo.Foundation.ProcessingEngine.Models;
using Sitecore.Processing.Engine.ML.Abstractions;

namespace Demo.Foundation.ProcessingEngine.Train.Models
{
    public class RfmStatistics: ModelStatistics
    {
        public List<Customer> Customers { get; set; }
    }
}
