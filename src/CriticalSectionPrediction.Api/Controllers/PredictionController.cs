// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using CriticalSectionPrediction.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;

namespace CriticalSectionPrediction.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PredictionController : ControllerBase
    {
        [HttpPost]
        [Route("/PredictWaitingTime")]
        public IActionResult Predict(ModelInput modelInput)
        {
            var mlContext = new MLContext();
            ITransformer model = mlContext.Model.Load("\\Data\\Simulation.zip", out DataViewSchema schema);
            var predictionFunction = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model);
            var prediction = predictionFunction.Predict(modelInput);

            // Check for ML model fitting error
            if(prediction.WaitingTime < 0)
            {
                return NotFound("Prediction not available");
            }

            return Ok(prediction);
        }
    }
}
