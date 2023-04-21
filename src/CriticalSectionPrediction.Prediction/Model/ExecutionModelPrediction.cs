// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.ML.Data;

namespace CriticalSectionPrediction.Prediction.Model
{
    /// <summary>
    /// The ML prediction model
    /// </summary>
    public class ExecutionModelPrediction
    {
        // WaitingTime column is the Label that you will predict
        [ColumnName("Score")]
        public float WaitingTime;
    }
}
