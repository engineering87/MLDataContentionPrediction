// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace CriticalSectionPrediction.Prediction.Entity
{
    public class Data
    {
        public string ResultFilePath { get; set; }
        public string TrainingFilePath { get; set; }
        public string TestFilePath { get; set; }
        public string SimulationFilePath { get; set; }
        public uint PredictionSimulationTime { get; set; }
    }
}
