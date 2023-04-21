// https://github.com/dotnet/machinelearning-samples/tree/main/samples/csharp/getting-started/Regression_TaxiFarePrediction
using Microsoft.ML.Data;
using Microsoft.ML;
using System.Diagnostics;

namespace CriticalSectionPrediction.Prediction.Helper
{
    public static class ConsoleHelper
    {
        private const int Width = 114;

        public static void PrintRegressionPredictionVersusObserved(string predictionCount, string observedCount)
        {
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine($"Predicted : {predictionCount}");
            Console.WriteLine($"Actual:     {observedCount}");
            Console.WriteLine("-------------------------------------------------");
        }

        public static void PrintRegressionMetrics(string name, RegressionMetrics metrics)
        {
            Console.WriteLine("*************************************************");
            Console.WriteLine($"*       Metrics for {name} regression model      ");
            Console.WriteLine("*------------------------------------------------");
            Console.WriteLine($"*       LossFn:        {metrics.LossFunction:0.##}");
            Console.WriteLine($"*       R2 Score:      {metrics.RSquared:0.##}");
            Console.WriteLine($"*       Absolute loss: {metrics.MeanAbsoluteError:#.##}");
            Console.WriteLine($"*       Squared loss:  {metrics.MeanSquaredError:#.##}");
            Console.WriteLine($"*       RMS loss:      {metrics.RootMeanSquaredError:#.##}");
            Console.WriteLine("*************************************************");
        }

        public static double CalculateStandardDeviation(IEnumerable<double> values)
        {
            var enumerable = values as double[] ?? values.ToArray();
            var average = enumerable.Average();
            var sumOfSquaresOfDifferences = enumerable.Select(val => (val - average) * (val - average)).Sum();
            var standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / (enumerable.Count() - 1));
            return standardDeviation;
        }

        public static void ShowDataViewInConsole(IDataView dataView, int numberOfRows = 5)
        {
            var msg = $"Show data in DataView: Showing {numberOfRows} rows with the columns";
            ConsoleWriteHeader(msg);

            var preViewTransformedData = dataView.Preview(maxRows: numberOfRows);

            foreach (var row in preViewTransformedData.RowView)
            {
                var columnCollection = row.Values;
                var lineToPrint = columnCollection.Aggregate("Row --> ", (current, column) => current + $"| {column.Key}:{column.Value}");
                Console.WriteLine(lineToPrint + "\n");
            }
        }

        /// <summary>
        /// Write strings to console
        /// </summary>
        /// <param name="lines"></param>
        public static void ConsoleWriteHeader(params string[] lines)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" ");
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
            var maxLength = lines.Select(x => x.Length).Max();
            Console.WriteLine(new string('#', maxLength));
            Console.ForegroundColor = defaultColor;
        }

        [Conditional("DEBUG")]
        // This method using 'DebuggerExtensions.Preview()' should only be used when debugging/developing, not for release/production trainings
        public static void PeekDataViewInConsole(MLContext mlContext, IDataView dataView, IEstimator<ITransformer> pipeline, int numberOfRows = 4)
        {
            string msg = string.Format("Peek data in DataView: Showing {0} rows with the columns", numberOfRows.ToString());
            ConsoleWriteHeader(msg);

            //https://github.com/dotnet/machinelearning/blob/main/docs/code/MlNetCookBook.md#how-do-i-look-at-the-intermediate-data
            var transformer = pipeline.Fit(dataView);
            var transformedData = transformer.Transform(dataView);

            // 'transformedData' is a 'promise' of data, lazy-loading. call Preview
            //and iterate through the returned collection from preview.

            var preViewTransformedData = transformedData.Preview(maxRows: numberOfRows);

            foreach (var row in preViewTransformedData.RowView)
            {
                var ColumnCollection = row.Values;
                string lineToPrint = "Row--> ";
                foreach (KeyValuePair<string, object> column in ColumnCollection)
                {
                    lineToPrint += $"| {column.Key}:{column.Value}";
                }
                Console.WriteLine(lineToPrint + "\n");
            }
        }

        [Conditional("DEBUG")]
        // This method using 'DebuggerExtensions.Preview()' should only be used when debugging/developing, not for release/production trainings
        public static void PeekVectorColumnDataInConsole(MLContext mlContext, string columnName, IDataView dataView, IEstimator<ITransformer> pipeline, int numberOfRows = 4)
        {
            string msg = string.Format("Peek data in DataView: : Show {0} rows with just the '{1}' column", numberOfRows, columnName);
            ConsoleWriteHeader(msg);

            var transformer = pipeline.Fit(dataView);
            var transformedData = transformer.Transform(dataView);

            // Extract the 'Features' column.
            var someColumnData = transformedData.GetColumn<float[]>(columnName)
                                                        .Take(numberOfRows).ToList();

            // print to console the peeked rows

            int currentRow = 0;
            someColumnData.ForEach(row => {
                currentRow++;
                String concatColumn = String.Empty;
                foreach (float f in row)
                {
                    concatColumn += f.ToString();
                }

                Console.WriteLine();
                string rowMsg = string.Format("**** Row {0} with '{1}' field value ****", currentRow, columnName);
                Console.WriteLine(rowMsg);
                Console.WriteLine(concatColumn);
                Console.WriteLine();
            });
        }

        public static void PrintIterationMetrics(int iteration, string trainerName, RegressionMetrics metrics, double? runtimeInSeconds)
        {
            CreateRow($"{iteration,-4} {trainerName,-35} {metrics?.RSquared ?? double.NaN,8:F4} {metrics?.MeanAbsoluteError ?? double.NaN,13:F2} {metrics?.MeanSquaredError ?? double.NaN,12:F2} {metrics?.RootMeanSquaredError ?? double.NaN,8:F2} {runtimeInSeconds.Value,9:F1}", Width);
        }

        public static void PrintIterationException(Exception ex)
        {
            Console.WriteLine($"Exception during AutoML iteration: {ex}");
        }

        public static void PrintIterationMetrics(int iteration, string trainerName, RankingMetrics metrics, double? runtimeInSeconds)
        {
            CreateRow($"{iteration,-4} {trainerName,-15} {metrics?.NormalizedDiscountedCumulativeGains[0] ?? double.NaN,9:F4} {metrics?.NormalizedDiscountedCumulativeGains[2] ?? double.NaN,9:F4} {metrics?.NormalizedDiscountedCumulativeGains[9] ?? double.NaN,9:F4} {metrics?.DiscountedCumulativeGains[9] ?? double.NaN,9:F4} {runtimeInSeconds.Value,9:F1}", Width);
        }

        public static void PrintRegressionMetricsHeader()
        {
            CreateRow($"{"",-4} {"Trainer",-35} {"RSquared",8} {"Absolute-loss",13} {"Squared-loss",12} {"RMS-loss",8} {"Duration",9}", Width);
        }

        private static void CreateRow(string message, int width)
        {
            Console.WriteLine("|" + message.PadRight(width - 2) + "|");
        }
    }
}
