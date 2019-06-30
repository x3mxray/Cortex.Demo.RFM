using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MLServer.Models;
using MLServer.Services;

namespace MLServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RfmController : ControllerBase
    {
       
        [HttpGet("test")]
        public bool Test()
        {
            return true;
        }

        [HttpPost("train")]
        public bool Train([FromBody] List<Rfm> data)
        {
            if (data != null && data.Any())
            {
                var segmentator = new CustomersSegmentator();
                segmentator.Train(data);
                return true;
            }

            return false;
        }

        [HttpPost("predict")]
        public List<int> PredictList([FromBody] List<ClusteringData> data)
        {
            var segmentator = new CustomersSegmentator();
            return segmentator.Predict(data);
        }


        [HttpPost("trainforecast")]
        public bool TrainForecast([FromBody] List<ProductStats> data)
        {
            if (data != null && data.Any())
            {
                var segmentator = new CustomersSegmentator();
                segmentator.TrainForecast(data);
                return true;
            }

            return false;
        }

        [HttpPost("productstats")]
        public List<ProductStats> ProductStats(string productId)
        {
            return CustomersSegmentator.ProductHistory(productId);
        }

        [HttpPost("productforecast")]
        public float ProductForecast(ProductStats data)
        {
            return CustomersSegmentator.ProductForecast(data);
        }

        [HttpPost("trainforecastcountry")]
        public bool TrainForecastCountry([FromBody] List<CountryStats> data)
        {
            if (data != null && data.Any())
            {
                var segmentator = new CustomersSegmentator();
                segmentator.TrainForecastCountry(data);
                return true;
            }

            return false;
        }

        [HttpPost("countrystats")]
        public List<CountryStats> CountryStats(string productId)
        {
            return CustomersSegmentator.CountryHistory(productId);
        }

        [HttpPost("countryforecast")]
        public float CountryForecast(CountryStats data)
        {
            return CustomersSegmentator.CountryForecast(data);
        }

        [HttpPost("getcountries")]
        public List<string> GetCountries()
        {
            return CustomersSegmentator.GetCountries();
        }
    }
}
