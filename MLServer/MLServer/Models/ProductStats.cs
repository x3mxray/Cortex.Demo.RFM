namespace MLServer.Models
{
    public class ProductStats
    {
        public string ProductId { get; set; }
        public float Year { get; set; }
        public float Month { get; set; }
        //public float Max { get; set; }
        //public float Min { get; set; }
        public float Avg { get; set; }
        public float Count { get; set; }

        public float Units { get; set; }
        public float Prev { get; set; }
        public float Next { get; set; }
    }

    public class CountryStats
    {
        public string Country { get; set; }
        public float Year { get; set; }
        public float Month { get; set; }
        //public float Max { get; set; }
        //public float Min { get; set; }
        public float Avg { get; set; }
        public float Count { get; set; }

        public float Units { get; set; }
        public float Prev { get; set; }
        public float Next { get; set; }
    }

    public class ProductUnitPrediction
    {
        public float Score;
    }
}
