// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using CriticalSectionPrediction.Simulation.Entity;
using CriticalSectionPrediction.Simulation.Enum;
using CriticalSectionPrediction.Simulation.Utility;
using System.Collections.Concurrent;

namespace CriticalSectionPrediction.Simulation.Simulation
{
    public static class SamplesCollector
    {
        public static ConcurrentBag<Sample> SimulationSamples = new ConcurrentBag<Sample>();

        public static void WriteSamples(SampleDestination sampleDestination)
        {
            switch(sampleDestination) 
            {
                case SampleDestination.CSV:
                    {
                        CsvWriter.WriteCsv(SimulationSamples);
                        break;
                    }
            }
        }
    }
}
