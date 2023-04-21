// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using CriticalSectionPrediction.Simulation.Simulation;

namespace CriticalSectionPrediction.Simulation.Entity
{
    public class TaskSimulation
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly CurrentSimulation _currentSimulation;

        public TaskSimulation(CurrentSimulation currentSimulation)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _currentSimulation = currentSimulation;
        }

        /// <summary>
        /// Start the current Task
        /// </summary>
        public void Start()
        {
            Dispatcher.DispatchTask(
                    _currentSimulation,
                    _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Stop the current Task
        /// </summary>
        public void Stop()
        {
            _cancellationTokenSource.Cancel(false);
        }
    }
}
