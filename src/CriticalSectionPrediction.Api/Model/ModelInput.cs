// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace CriticalSectionPrediction.Api.Model
{
    public class ModelInput
    {
        public float Thread { get; set; }
        public float TimeOnSection { get; set; }
        public float CriticalSectionDimension { get; set; }
        public float WaitingTime { get; set; }
    }
}
