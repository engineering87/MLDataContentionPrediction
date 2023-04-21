// https://github.com/dotnet/machinelearning-samples/tree/main/samples/csharp/getting-started/Regression_TaxiFarePrediction
using CriticalSectionPrediction.Prediction.Model;
using Microsoft.ML;
using PLplot;
using System.Diagnostics;
using System.Globalization;

namespace CriticalSectionPrediction.Prediction
{
    public class PlotManager
    {
        /// <summary>
        /// Plot the WaitingTime distribution plot
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="testDataSetPath"></param>
        /// <param name="simulationZipPath"></param>
        /// <param name="numberOfRecordsToRead"></param>
        /// <param name="args"></param>
        public static void PlotRegressionChart(MLContext mlContext,
                                                string testDataSetPath,
                                                string simulationZipPath,
                                                int numberOfRecordsToRead,
                                                string[] args)
        {
            ITransformer trainedModel;
            using (var stream = new FileStream(simulationZipPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                trainedModel = mlContext.Model.Load(stream, out var modelInputSchema);
            }

            // Create prediction engine related to the loaded trained model
            var predFunction = mlContext.Model.CreatePredictionEngine<ExecutionModel, ExecutionModelPrediction>(trainedModel);

            string chartFileName = "";
            using (var pl = new PLStream())
            {
                if (args.Length == 1 && args[0] == "svg")
                {
                    pl.sdev("svg");
                    chartFileName = "WaitingTimeDistribution.svg";
                    pl.sfnam(chartFileName);
                }
                else
                {
                    pl.sdev("pngcairo");
                    chartFileName = "WaitingTimeDistribution.png";
                    pl.sfnam(chartFileName);
                }

                // use white background with black foreground
                pl.spal0("cmap0_alternate.pal");

                // Initialize plplot
                pl.init();

                // set axis limits
                const int xMinLimit = 0;
                const int xMaxLimit = 1000;
                const int yMinLimit = 0;
                const int yMaxLimit = 1000;
                pl.env(xMinLimit, xMaxLimit, yMinLimit, yMaxLimit, AxesScale.Independent, AxisBox.BoxTicksLabelsAxes);

                // Set scaling for mail title text 125% size of default
                pl.schr(0, 1.25);

                // The main title
                pl.lab("Measured", "Predicted", "Distribution of WaitingTime Prediction");

                // plot using different colors
                // see http://plplot.sourceforge.net/examples.php?demo=02 for palette indices
                pl.col0(1);

                int totalNumber = numberOfRecordsToRead;
                var testData = GetDataFromCsv(testDataSetPath, totalNumber).ToList();

                //This code is the symbol to paint
                char code = (char)9;

                // plot using other color
                //pl.col0(9); //Light Green
                //pl.col0(4); //Red
                pl.col0(2); //Blue

                double yTotal = 0;
                double xTotal = 0;
                double xyMultiTotal = 0;
                double xSquareTotal = 0;

                for (int i = 0; i < testData.Count; i++)
                {
                    var x = new double[1];
                    var y = new double[1];

                    //Make Prediction
                    var waitingTimePrediction = predFunction.Predict(testData[i]);

                    x[0] = testData[i].WaitingTime;
                    y[0] = waitingTimePrediction.WaitingTime;

                    //Paint a dot
                    pl.poin(x, y, code);

                    xTotal += x[0];
                    yTotal += y[0];

                    double multi = x[0] * y[0];
                    xyMultiTotal += multi;

                    double xSquare = x[0] * x[0];
                    xSquareTotal += xSquare;

                    double ySquare = y[0] * y[0];

                    Console.WriteLine($"-------------------------------------------------");
                    Console.WriteLine($"Predicted : {waitingTimePrediction.WaitingTime}");
                    Console.WriteLine($"Actual:    {testData[i].WaitingTime}");
                    Console.WriteLine($"-------------------------------------------------");
                }

                // Regression Line calculation explanation:
                // https://www.khanacademy.org/math/statistics-probability/describing-relationships-quantitative-data/more-on-regression/v/regression-line-example

                double minY = yTotal / totalNumber;
                double minX = xTotal / totalNumber;
                double minXY = xyMultiTotal / totalNumber;
                double minXsquare = xSquareTotal / totalNumber;

                double m = ((minX * minY) - minXY) / ((minX * minX) - minXsquare);

                double b = minY - (m * minX);

                //Generic function for Y for the regression line
                // y = (m * x) + b;

                double x1 = 1;
                //Function for Y1 in the line
                double y1 = (m * x1) + b;

                double x2 = 1000;
                //Function for Y2 in the line
                double y2 = (m * x2) + b;

                var xArray = new double[2];
                var yArray = new double[2];
                xArray[0] = x1;
                yArray[0] = y1;
                xArray[1] = x2;
                yArray[1] = y2;

                pl.col0(4);
                pl.line(xArray, yArray);

                // end page (writes output to disk)
                pl.eop();

                // output version of PLplot
                pl.gver(out var verText);
                Console.WriteLine("PLplot version " + verText);

            } // the pl object is disposed here

            // Open Chart File In Microsoft Photos App (Or default app, like browser for .svg)

            Console.WriteLine("Showing chart...");
            var p = new Process();
            string chartFileNamePath = @".\" + chartFileName;
            p.StartInfo = new ProcessStartInfo(chartFileNamePath)
            {
                UseShellExecute = true
            };
            p.Start();
        }

        /// <summary>
        /// Get min and max of WaitingTime
        /// </summary>
        /// <param name="testData"></param>
        /// <returns></returns>
        private static Tuple<float, float> GetMinMax(IEnumerable<ExecutionModel> testData)
        {
            var executionModels = testData.ToList();
            var max = executionModels[0].WaitingTime;
            var min = executionModels[0].WaitingTime;

            foreach (var executionModel in executionModels)
            {
                if (executionModel.WaitingTime > max)
                    max = executionModel.WaitingTime;

                if (executionModel.WaitingTime < min)
                    min = executionModel.WaitingTime;
            }

            return new Tuple<float, float>(min, max);
        }

        private static IEnumerable<ExecutionModel> GetDataFromCsv(string dataLocation, int numMaxRecords)
        {
            try
            {
                var records =
                    File.ReadAllLines(dataLocation)
                        .Skip(1)
                        .Select(x => x.Split(','))
                        .Select(x => new ExecutionModel()
                        {
                            Thread = float.Parse(x[0], CultureInfo.InvariantCulture),
                            TimeOnSection = float.Parse(x[1], CultureInfo.InvariantCulture),
                            CriticalSectionDimension = float.Parse(x[2], CultureInfo.InvariantCulture),
                            WaitingTime = float.Parse(x[3], CultureInfo.InvariantCulture)
                        }).Take<ExecutionModel>(numMaxRecords);

                return records;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
