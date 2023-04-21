// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace CriticalSectionPrediction.Simulation.Entity
{
    public class Sample
    {
        public CurrentSimulation CurrentSimulation { get; private set; }
        public long WaitingTime { get; private set; }

        public Sample(CurrentSimulation CurrentSimulation, long WaitingTime)
        {
            this.CurrentSimulation = CurrentSimulation;
            this.WaitingTime = WaitingTime;
        }
    }
}
