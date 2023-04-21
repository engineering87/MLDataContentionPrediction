// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.ML.Data;

namespace CriticalSectionPrediction.Api.Model
{
    public class ModelOutput
    {
        [ColumnName("Score")]
        public float WaitingTime { get; set; }
    }
}
