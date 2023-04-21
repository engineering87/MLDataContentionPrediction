// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using CriticalSectionPrediction.Simulation.Entity;
using System.Collections.Concurrent;

namespace CriticalSectionPrediction.Simulation.Utility
{
    public class CsvWriter
    {
        /// <summary>
        /// TODO move to Helper project
        /// </summary>
        private const string _resultPath = @"\Data";
        private const string _resultFileName = @"\Result.csv";

        public static void WriteCsv(ConcurrentBag<Sample> simulationSamples)
        {
            try
            {
                bool exists = Directory.Exists(_resultPath);

                if (!exists)
                    Directory.CreateDirectory(_resultPath);

                foreach (var sample in simulationSamples)
                {
                    var currentSimulation = sample.CurrentSimulation;
                    var line = $"{currentSimulation.ThreadNumber}," +
                        $"{currentSimulation.TimeOnSection}," +
                        $"{currentSimulation.CriticalSectionDimension}," +
                        $"{sample.WaitingTime}";
                    line += Environment.NewLine;

                    File.AppendAllText(string.Concat(_resultPath, _resultFileName), line);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
