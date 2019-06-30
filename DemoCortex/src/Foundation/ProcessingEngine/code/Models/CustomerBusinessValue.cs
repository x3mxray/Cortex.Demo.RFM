namespace Demo.Foundation.ProcessingEngine.Models
{
    public class CustomerBusinessValue 
    {
       
        public double Recency { get; set; }
        public int Frequency { get; set; }
        public decimal Monetary { get; set; }
        public int R { get; set; }
        public int F { get; set; }
        public int M { get; set; }
    }

    public class Rfm
    {
        public float R { get; set; }
        public float F { get; set; }
        public float M { get; set; }
    }
}
