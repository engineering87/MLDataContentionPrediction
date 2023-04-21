// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace CriticalSectionPrediction.Simulation.Entity
{
    public class CurrentSimulation
    {
        /// <summary>
        /// Simulation profile parameters
        /// </summary>
        public int ThreadNumber { get; set; }
        public int CriticalSectionDimension { get; set; }
        public int TimeOnSection { get; set; }
        /// <summary>
        /// The total samples to retrieve
        /// </summary>
        public int TotalSamples { get; set; }
    }
}
