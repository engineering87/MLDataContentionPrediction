// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using CriticalSectionPrediction.Orchestrator;
using CriticalSectionPrediction.Orchestrator.Configuration;
using CriticalSectionPrediction.Prediction.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var configuration = new ConfigurationBuilder()
     .SetBasePath(Directory.GetCurrentDirectory())
     .AddJsonFile($"appsettings.json")
     .AddEnvironmentVariables();

var config = configuration.Build();

using ILoggerFactory loggerFactory =
    LoggerFactory.Create(builder =>
        builder.AddConsole());

ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

logger.LogInformation($"Starting CriticalSectionPrediction orchestrator {typeof(Program).Assembly.GetName().Version}");

var settings = config.GetSection("Configuration").Get<Config>();

if(settings == null)
{
    throw new ArgumentNullException("Configuration is empty");
}

var data = Mapping.Mapper.Map<Data>(settings);

var orchestratorManager = new OrchestratorManager(settings, data);
orchestratorManager.StartOrchestration();

Console.ReadLine();

logger.LogInformation($"Shutting down CriticalSectionPrediction orchestrator");