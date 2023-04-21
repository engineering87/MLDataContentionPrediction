// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Text.Json;

namespace CriticalSectionPrediction.Simulation.Entity
{
    public class Profile
    {
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

        /// <summary>
        /// ToString override for JSON output
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
