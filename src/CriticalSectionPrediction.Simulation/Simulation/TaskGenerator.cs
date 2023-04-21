// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using CriticalSectionPrediction.Simulation.Entity;

namespace CriticalSectionPrediction.Simulation.Simulation
{
    public class TaskGenerator
    {
        private readonly List<TaskSimulation> _threadsList;
        private readonly CurrentSimulation _currentSimulation;

        public TaskGenerator(CurrentSimulation currentSimulation)
        {
            _currentSimulation = currentSimulation;
            _threadsList = new List<TaskSimulation>();

            for (var i = 0; i < currentSimulation.ThreadNumber; i++)
            {
                var thread = new TaskSimulation(currentSimulation);
                _threadsList.Add(thread);
            }
        }

        /// <summary>
        /// Stop all the Task
        /// </summary>
        public void StopTask()
        {
            foreach (var thread in _threadsList)
            {
                thread.Stop();
            }
        }

        /// <summary>
        /// Start all the Task
        /// </summary>
        public void StartTask()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            ThreadPool.SetMinThreads(_threadsList.Count, _threadsList.Count);

            Task[] tasks = Enumerable.Range(0, _threadsList.Count)
                .Select(_ => Task.Run(() => Dispatcher.DispatchTask(
                        _currentSimulation,
                        cancellationTokenSource.Token)))
                .ToArray();

            Task.WaitAll(tasks);
        }
    }
}
