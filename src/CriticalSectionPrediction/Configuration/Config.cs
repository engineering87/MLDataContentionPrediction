// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace CriticalSectionPrediction.Orchestrator.Configuration
{
    /// <summary>
    /// Global configuration
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Simulation parameters
        /// </summary>
        public int MinThread { get; set; }
        public int MaxThread { get; set; }
        public int MinTimeOnSection { get; set; }
        public int MaxTimeOnSection { get; set; }
        public int MinCriticalSectionDimension { get; set; }
        public int MaxCriticalSectionDimension { get; set; }
        public uint PredictionSimulationTime { get; set; }
        public int TotalSimulationDimension { get; set; }
        public int PartialSimulationDimension { get; set; }
        public int TrainingSetPercentage { get; set; }
        public int TotalSamples { get; set; }
        /// <summary>
        /// Data files path
        /// </summary>
        public string ResultFilePath { get; set; }
        public string TrainingFilePath { get; set; }
        public string TestFilePath { get; set; }
        public string SimulationFilePath { get; set; }
    }
}
