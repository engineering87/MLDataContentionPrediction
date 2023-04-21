// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using CriticalSectionPrediction.Simulation.Entity;
using System.Diagnostics;

namespace CriticalSectionPrediction.Simulation.Simulation
{
    public class Dispatcher
    {
        /// <summary>
        /// Dispath the current Task to the critical section
        /// </summary>
        /// <param name="currentSimulation"></param>
        /// <param name="cancellationToken"></param>
        public static void DispatchTask(
            CurrentSimulation currentSimulation,
            CancellationToken cancellationToken)
        {
            var samples = 0;

            while (true)
            {
                // check for samples complete
                if (currentSimulation.TotalSamples == samples)
                {
                    return;
                }

                var stopwatch = Stopwatch.StartNew();

                CriticalSection
                    .GetInstance(currentSimulation.CriticalSectionDimension)
                    .UseSection(currentSimulation, cancellationToken);

                stopwatch.Stop();

                // compute the waiting time, time to get an available slot on the Critical Section
                // Wt = waiting time
                // Dt = dispatcher time
                // St = section time
                // Wt = Dt - St
                var waitingTime = stopwatch.ElapsedMilliseconds == 0 ? 0 : stopwatch.ElapsedMilliseconds - currentSimulation.TimeOnSection;
                Console.WriteLine($"Task Id {Task.CurrentId} - Waiting time {waitingTime} ms");

                // add the current sample to the collector
                SamplesCollector.SimulationSamples.Add(new Sample(currentSimulation, waitingTime));

                samples++;
            }
        }
    }
}
