// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using CriticalSectionPrediction.Orchestrator.Configuration;
using CriticalSectionPrediction.Simulation.Entity;
using CriticalSectionPrediction.Simulation;
using CriticalSectionPrediction.Prediction;
using CriticalSectionPrediction.Prediction.Entity;
using CriticalSectionPrediction.Prediction.Helper;
using CriticalSectionPrediction.Helper.File;

namespace CriticalSectionPrediction.Orchestrator
{
    public class OrchestratorManager
    {
        private readonly Config _configuration;
        private readonly Data _data;

        public OrchestratorManager(Config configuration, Data data)
        {
            _configuration = configuration;
            _data = data;
        }

        /// <summary>
        /// Entry point for the ML project orchestrator
        /// </summary>
        public void StartOrchestration()
        {
            // check for the last simulation
            if (File.Exists(_configuration.ResultFilePath))
            {
                ConsoleHelper.ConsoleWriteHeader("===== Reset the last simulation? Y/N =====");
                var userResponse = Console.ReadLine();
                if (userResponse?.ToLower() == "y" || userResponse?.ToLower() == "yes")
                {
                    FileUtility.ResetSimulationFile(_configuration.ResultFilePath);
                    StartSimulation();
                }
            }
            else
            {
                StartSimulation();
            }

            // check for the last machine learning model
            if (File.Exists(_configuration.TrainingFilePath))
            {
                ConsoleHelper.ConsoleWriteHeader("===== Reset the last ML model? Y/N =====");
                var userResponse = Console.ReadLine();
                if (userResponse?.ToLower() == "y" || userResponse?.ToLower() == "yes")
                {
                    FileUtility.ResetModelFile(
                        _configuration.TrainingFilePath,
                        _configuration.TestFilePath,
                        _configuration.SimulationFilePath);
                    BuildMachineLearningSet();
                }
            }
            else
            {
                BuildMachineLearningSet();
            }

            ConsoleHelper.ConsoleWriteHeader("===== Starting prediction =====");
            var predictionManager = new PredictionManager(_data);
            predictionManager.StartPrediction();
        }

        /// <summary>
        /// Start the simulation and collect samples
        /// </summary>
        private void StartSimulation()
        {
            ConsoleHelper.ConsoleWriteHeader("===== Starting simulation =====");

            var simulationDimension = 0;
            while (simulationDimension <= _configuration.TotalSimulationDimension)
            {
                // iterate over task configuration
                for (var t = _configuration.MinThread;
                    t <= _configuration.MaxThread;
                    t += 2)
                {
                    // iterate over critical section dimension
                    for (var c = _configuration.MinCriticalSectionDimension;
                        c <= _configuration.MaxCriticalSectionDimension;
                        c += 2)
                    {
                        // iterate over time on critical section
                        for (var w = _configuration.MinTimeOnSection;
                            w <= _configuration.MaxTimeOnSection;
                            w += 100)
                        {
#if DEBUG
                            Console.WriteLine(
                                $"SimulateConfiguration " +
                                $"thread={t} " +
                                $"criticalSectionDimension={c} " +
                                $"criticalSectionTime={w}");
#endif
                            // iterate over the partial step
                            for (var p = 0;
                                p <= _configuration.PartialSimulationDimension;
                                p++)
                            {
                                // skip if task number is > critical section slot number
                                if (t > c)
                                {
                                    // avoid starvation
                                    continue;
                                }

                                var currentSimulation = new CurrentSimulation()
                                {
                                    ThreadNumber = t,
                                    CriticalSectionDimension = c,
                                    TimeOnSection = w,
                                    TotalSamples = _configuration.TotalSamples
                                };

                                var simulationManager = new SimulationManager(currentSimulation);
                                simulationManager.StartSimulation();

                                simulationDimension++;
                            }
                        }
                    }
                }

                // simulation.TotalSimulationDimension < possible configurations
                break;
            }
        }

        /// <summary>
        /// Build the machine learning training and test set
        /// </summary>
        private void BuildMachineLearningSet()
        {
            ConsoleHelper.ConsoleWriteHeader("===== Building the ML training and test set =====");

            // total number of samples from simulation
            var resultCount = File.ReadLines(_configuration.ResultFilePath).Count();
            // total number of sample for the training set
            var trainingSetCount = resultCount * (_configuration.TrainingSetPercentage / 100D);
            var trainingStep = (int)(resultCount / trainingSetCount);
            // init empty set
            var trainingSet = new List<string>();
            var testingSet = new List<string>();

            using (var reader = new StreamReader(_configuration.ResultFilePath))
            {
                var step = 0;
                while (!reader.EndOfStream)
                {
                    if (step % trainingStep == 0 && trainingSet.Count <= trainingSetCount)
                    {
                        trainingSet.Add(reader.ReadLine() ?? string.Empty);
                    }
                    else
                    {
                        testingSet.Add(reader.ReadLine() ?? string.Empty);
                    }
                    step++;
                }
            }

            CsvWriter.WriteTrainingSet(trainingSet, _configuration.TrainingFilePath);
            CsvWriter.WriteTestSet(testingSet, _configuration.TestFilePath);
        }        
    }
}
