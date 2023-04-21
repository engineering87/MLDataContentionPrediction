// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace CriticalSectionPrediction.Helper.File
{
    public static class CsvWriter
    {
        /// <summary>
        /// Write the CSV file for the training data set
        /// </summary>
        /// <param name="trainingSet"></param>
        /// <param name="trainingFilePath"></param>
        public static void WriteTrainingSet(IEnumerable<string> trainingSet, string trainingFilePath)
        {
            using (var file = new StreamWriter(trainingFilePath, true))
            {
                file.WriteLine("Thread,TimeOnSection,CriticalSectionDimension,WaitingTime");
                foreach (var line in trainingSet)
                {
                    file.WriteLine($"{line}");
                }
            }
        }

        /// <summary>
        /// Write the CSV file for the test data set
        /// </summary>
        /// <param name="testingSet"></param>
        /// <param name="testFilePath"></param>
        public static void WriteTestSet(IEnumerable<string> testingSet, string testFilePath)
        {
            using (var file = new StreamWriter(testFilePath, true))
            {
                file.WriteLine("Thread,TimeOnSection,CriticalSectionDimension,WaitingTime");
                foreach (var line in testingSet)
                {
                    file.WriteLine($"{line}");
                }
            }
        }
    }
}
