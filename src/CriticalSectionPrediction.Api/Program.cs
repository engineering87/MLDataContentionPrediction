// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using CriticalSectionPrediction.Api.Model;
using Microsoft.Extensions.ML;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddPredictionEnginePool<ModelInput, ModelOutput>()
//    .FromFile(modelName: "PredictionModel", filePath: "\\Data\\Simulation.zip", watchForChanges: true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

//var predictionHandler =
//    async (PredictionEnginePool<ModelInput, ModelOutput> predictionEnginePool, ModelInput input) =>
//        await Task.FromResult(predictionEnginePool.Predict(modelName: "PredictionModel", input));

//app.MapPost("/predict", predictionHandler);

app.Run();
