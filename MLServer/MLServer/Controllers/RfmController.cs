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

    }
}
