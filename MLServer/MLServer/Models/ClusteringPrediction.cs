using Microsoft.ML.Data;

namespace MLServer.Models
{
    public class ClusteringPrediction
    {
        [ColumnName("PredictedLabel")]
        public uint SelectedClusterId;

        [ColumnName("Score")]
        public float[] Distance;
    }
}
