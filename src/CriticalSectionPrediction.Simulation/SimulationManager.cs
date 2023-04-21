// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using CriticalSectionPrediction.Simulation.Entity;
using CriticalSectionPrediction.Simulation.Enum;
using CriticalSectionPrediction.Simulation.Simulation;

namespace CriticalSectionPrediction.Simulation
{
    public class SimulationManager : ISimulationManager
    {
        private readonly TaskGenerator _threadGenerator;

        public SimulationManager(CurrentSimulation currentSimulation)
        {
            _threadGenerator = new TaskGenerator(currentSimulation);
        }

        public void StartSimulation()
        {
            // start the current simulation
            _threadGenerator.StartTask();

            // write samples
            SamplesCollector.WriteSamples(SampleDestination.CSV);
        }

        public void StopSimulation()
        {
            _threadGenerator.StopTask();
        }
    }
}
