using CriticalSectionPrediction.Prediction.Helper;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;

namespace CriticalSectionPrediction.Prediction.Common
{
    /// <summary>
    /// Progress handler that AutoML will invoke after each model it produces and evaluates.
    /// </summary>
    public class RegressionExperimentProgressHandler : IProgress<RunDetail<RegressionMetrics>>
    {
        private int _iterationIndex;

        public void Report(RunDetail<RegressionMetrics> iterationResult)
        {
            if (_iterationIndex++ == 0)
            {
                ConsoleHelper.PrintRegressionMetricsHeader();
            }

            if (iterationResult.Exception != null)
            {
                ConsoleHelper.PrintIterationException(iterationResult.Exception);
            }
            else
            {
                ConsoleHelper.PrintIterationMetrics(_iterationIndex, iterationResult.TrainerName,
                    iterationResult.ValidationMetrics, iterationResult.RuntimeInSeconds);
            }
        }
    }
}
