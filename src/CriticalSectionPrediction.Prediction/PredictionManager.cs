// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using CriticalSectionPrediction.Prediction.Entity;
using CriticalSectionPrediction.Prediction.Model;
using CriticalSectionPrediction.Prediction.Helper;
using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using CriticalSectionPrediction.Prediction.Common;

namespace CriticalSectionPrediction.Prediction
{
    public class PredictionManager : IPredictionManager
    {
        private readonly Data _data;

        public PredictionManager(Data data)
        {
            _data = data;
        }

        public void StartPrediction()
        {
            // Create ML Context with seed for repeatable/deterministic results
            var mlContext = new MLContext(seed: 0);

            // Create, Train, Evaluate and Save a model
            //BuildTrainEvaluateAndSaveModel(mlContext);

            // Create, Train, Evaluate and Save a model with AutoML
            AutoBuildTrainEvaluateAndSaveModel(mlContext);

            // Paint regression distribution chart for a number of elements read from a Test DataSet file
            PlotManager.PlotRegressionChart(
                mlContext, 
                _data.TestFilePath, 
                _data.SimulationFilePath, 
                1000, 
                Array.Empty<string>());
        }

        /// <summary>
        /// Train and test the ML model
        /// </summary>
        /// <param name="mlContext"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        private ITransformer BuildTrainEvaluateAndSaveModel(MLContext mlContext)
        {
            // STEP 1: Common data loading configuration
            // Load the training set and test set into the IDataView
            var baseTrainingDataView = mlContext.Data.LoadFromTextFile<ExecutionModel>(_data.TrainingFilePath, hasHeader: true, separatorChar: ',');
            var testDataView = mlContext.Data.LoadFromTextFile<ExecutionModel>(_data.TestFilePath, hasHeader: true, separatorChar: ',');

            ConsoleHelper.ShowDataViewInConsole(baseTrainingDataView);

            // (OPTIONAL) Sample code of removing extreme data like "outliers" for WaitingTime higher than 2000ms and lower than 1 which can be error-data 
            var trainingSetCount = baseTrainingDataView.GetColumn<float>(nameof(ExecutionModel.WaitingTime)).Count();
            IDataView trainingDataView = mlContext.Data.FilterRowsByColumn(baseTrainingDataView, nameof(ExecutionModel.WaitingTime), lowerBound: 1, upperBound: 2000);
            var filteredTrainingSetCount = trainingDataView.GetColumn<float>(nameof(ExecutionModel.WaitingTime)).Count();

            // STEP 2: Common data process configuration with pipeline data transformations
            var dataProcessPipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(ExecutionModel.WaitingTime))
                                        .Append(mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(ExecutionModel.Thread)))
                                        .Append(mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(ExecutionModel.TimeOnSection)))
                                        .Append(mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(ExecutionModel.CriticalSectionDimension)))
                                        .Append(mlContext.Transforms.Concatenate("Features", nameof(ExecutionModel.Thread), nameof(ExecutionModel.TimeOnSection), nameof(ExecutionModel.CriticalSectionDimension)));

            // (OPTIONAL) Peek data (such as 5 records) in training DataView after applying the ProcessPipeline's transformations into "Features" 
            ConsoleHelper.PeekDataViewInConsole(mlContext, trainingDataView, dataProcessPipeline, 5);
            ConsoleHelper.PeekVectorColumnDataInConsole(mlContext, "Features", trainingDataView, dataProcessPipeline, 5);

            // STEP 3: Set the training algorithm, then create and config the modelBuilder - Selected Trainer (SDCA Regression algorithm)  
            var trainer = mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", featureColumnName: "Features");
            var trainingPipeline = dataProcessPipeline.Append(trainer);

            // STEP 4: Train the model fitting to the DataSet
            // The pipeline is trained on the dataset that has been loaded and transformed.
            ConsoleHelper.ConsoleWriteHeader("=============== Training the model ===============");
            var trainedModel = trainingPipeline.Fit(trainingDataView);

            // STEP 5: Evaluate the model and show accuracy stats
            Console.WriteLine("===== Evaluating Model's accuracy with Test data =====");
            IDataView predictions = trainedModel.Transform(testDataView);
            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: "Label", scoreColumnName: "Score");

            ConsoleHelper.PrintRegressionMetrics(trainer.ToString(), metrics);

            // STEP 6: Save/persist the trained model to a .ZIP file
            using (var file = File.OpenWrite(_data.SimulationFilePath))
            {
                mlContext.Model.Save(trainedModel, trainingDataView.Schema, file);
            }

            return trainedModel;
        }

        /// <summary>
        /// Train and test the ML model with AutoML
        /// </summary>
        /// <param name="mlContext"></param>
        /// <returns></returns>
        private ITransformer AutoBuildTrainEvaluateAndSaveModel(MLContext mlContext)
        {
            // STEP 1: Common data loading configuration
            IDataView trainingDataView = mlContext.Data.LoadFromTextFile<ExecutionModel>(_data.TrainingFilePath, hasHeader: true, separatorChar: ',');
            IDataView testDataView = mlContext.Data.LoadFromTextFile<ExecutionModel>(_data.TestFilePath, hasHeader: true, separatorChar: ',');

            // STEP 2: Display first few rows of the training data
            ConsoleHelper.ShowDataViewInConsole(trainingDataView);

            // STEP 3: Initialize our user-defined progress handler that AutoML will 
            // invoke after each model it produces and evaluates.
            var progressHandler = new RegressionExperimentProgressHandler();

            // STEP 4: Run AutoML regression experiment
            // Instantiate and run an AutoML experiment. In doing so, specify how long the experiment should run in seconds (ExperimentTime),
            // and set a progress handler that will receive notifications after AutoML trains & evaluates each new model.
            ConsoleHelper.ConsoleWriteHeader("=============== Training the model ===============");
            Console.WriteLine($"Running AutoML regression experiment for {_data.PredictionSimulationTime} seconds...");
            ExperimentResult<RegressionMetrics> experimentResult = mlContext.Auto()
                .CreateRegressionExperiment(_data.PredictionSimulationTime)
                .Execute(trainingDataView, nameof(ExecutionModel.WaitingTime), progressHandler: progressHandler);

            // STEP 5: Evaluate the model and print metrics
            ConsoleHelper.ConsoleWriteHeader("===== Evaluating model's accuracy with test data =====");
            RunDetail<RegressionMetrics> best = experimentResult.BestRun;
            ITransformer trainedModel = best.Model;
            IDataView predictions = trainedModel.Transform(testDataView);
            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: nameof(ExecutionModel.WaitingTime), scoreColumnName: "Score");
            // Print metrics from top model
            ConsoleHelper.PrintRegressionMetrics(best.TrainerName, metrics);

            // STEP 6: Save/persist the trained model to a .ZIP file
            mlContext.Model.Save(trainedModel, trainingDataView.Schema, _data.SimulationFilePath);

            Console.WriteLine("The model is saved to {0}", _data.SimulationFilePath);

            return trainedModel;
        }
    }
}