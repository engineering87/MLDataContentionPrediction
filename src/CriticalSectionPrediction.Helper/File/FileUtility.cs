// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace CriticalSectionPrediction.Helper.File
{
    public static class FileUtility
    {
        public static void DeleteFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }

        public static void ResetSimulationFile(string simulationFilePath)
        {
            DeleteFile(simulationFilePath);
        }

        public static void ResetModelFile(
            string trainingFilePath, 
            string testFilePath, 
            string simulationFilePath)
        {
            DeleteFile(trainingFilePath);
            DeleteFile(testFilePath);
            DeleteFile(simulationFilePath);
        }
    }
}
