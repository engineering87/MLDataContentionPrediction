// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using CriticalSectionPrediction.Simulation.Entity;

namespace CriticalSectionPrediction.Simulation.Simulation
{
    public class CriticalSection
    {
        private static CriticalSection? _instance = null;
        private static readonly object syncRoot = new();
        private readonly List<object>? _criticalSectionList = null;

        public static CriticalSection GetInstance(int dimension)
        {
            if (_instance == null)
            {
                lock (syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new CriticalSection(dimension);
                    }
                }
            }
            return _instance;
        }

        private CriticalSection(int dimension)
        {
            _criticalSectionList = new List<object>();
            for (var i = 0; i < dimension; i++)
            {
                _criticalSectionList.Add(new object());
            }
        }

        /// <summary>
        /// Iterate over the critical section to find an available slot
        /// </summary>
        /// <param name="currentSimulation">The current simulation profile</param>
        /// <param name="cancellationToken">The cancellation token</param>
        public void UseSection(
            CurrentSimulation currentSimulation,
            CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    // get the first critical section available slot
                    for (var i = 0; i < _criticalSectionList?.Count; i++)
                    {
                        // check for thread cancellation request
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        var obj = _criticalSectionList[i];
                        if (Monitor.TryEnter(obj))
                        {
                            // slot found
                            // simulate blocking work
                            Thread.Sleep(currentSimulation.TimeOnSection);
                            // release the current slot and exit
                            Monitor.Exit(obj);
                            return;
                        }
                        else
                        {
                            // busy slot
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
