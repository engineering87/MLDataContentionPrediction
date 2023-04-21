// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.ML.Data;

namespace CriticalSectionPrediction.Prediction.Model
{
    /// <summary>
    /// The ML model for prediction
    /// </summary>
    public class ExecutionModel
    {
        [LoadColumn(0)]
        public float Thread;

        [LoadColumn(1)]
        public float TimeOnSection;

        [LoadColumn(2)]
        public float CriticalSectionDimension;

        [LoadColumn(3)]
        public float WaitingTime;
    }
}
