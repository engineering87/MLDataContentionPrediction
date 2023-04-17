# A Machine Learning model to predict Data Contention

This is a project developed in .NET 6 and ML.NET library for the analysis and prediction of data contention in lock based system by varying different parameters, such as the number of concurrent tasks, the critical section size and lock execution time. The final purpose is to predict the waiting time of a task to access the critical section.

### How it works
The project consists of three distinct modules: a data contention simulation module, which can be parameterized to simulate different application contexts; a module for generating the predictive model using ML.NET library; a module for managing and orchestrating simulations and machine learning models.

### Architecture
<img width="600" src="https://github.com/engineering87/MLDataContentionPrediction/blob/main/source2.png" style="vertical-align:middle">

### The Case Study
The case study is a didactic simulation of the data contention in a concurrent environment. The critical section is represented as a circular list of objects, a set of tasks tries to access the critical section by scanning the list in search of a non-locked slot.

### Model hypothesis
For the experiment we consider the following assumptions:

* The execution within the single slot of the critical section is of fixed duration and equal for each task
* The number of tasks is no greater than the number of slots available within the critical section
* The critical section scan time is 0

So we have:

* $𝑤_(k, d)$ represents the waiting time
* $𝐷_(k,𝑑)$ represents dispatching time
* $𝑆$ represents the time the slot is occupied
* $𝑤_(𝑘,𝑑)=𝐷_(𝑘,𝑑)−𝑆$
* $𝐷≥𝐾$

### How to configure it
The simulation and machine learning model configurations are fully described in the JSON *appsettings.json* file within the orchestrator's project.

```json
{
  "Configuration": {
    "ResultFilePath": "\\Data\\Result.csv",
    "TrainingFilePath": "\\Data\\Training.csv",
    "TestFilePath": "\\Data\\Test.csv",
    "SimulationFilePath": "\\Data\\Simulation.zip",
    "MinThread": "2",
    "MaxThread": "10",
    "MinTimeOnSection": "100",
    "MaxTimeOnSection": "500",
    "MinCriticalSectionDimension": "2",
    "MaxCriticalSectionDimension": "14",
    "PredictionSimulationTime": "20",
    "TotalSimulationDimension": "100",
    "PartialSimulationDimension": "10",
    "TrainingSetPercentage": "70",
    "TotalSamples":  "2"
  }
}
```

In the configuration file you can change any parameter of the simulation as you like, such as the task number, the interval of critical section size and the interval of lock execution time. It is also possible to modify the number of total samples to collect, the training time of the machine learning model and the size, in percentage, of the training set.

### Usage
To start the project, simply launch the **CriticalSectionOrchestrator** project on VS after having appropriately configured the parameters for the simulation in the JSON file.

### Result
At the end of the simulation and prediction, the distribution graph will be show to assess the reliability of the model obtained. The ML model will be compared with the simulation results in terms of waiting time.



### ML.NET
ML.NET is an open source and cross-platform machine learning framework, below the references of the project:
 * [MainSite] (https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet)
 * [GitHub] (https://github.com/dotnet/machinelearning)

### Contributing
Thank you for considering to help out with the source code! We welcome contributions from anyone on the internet, and are grateful for even the smallest of fixes!
If you'd like to contribute, please fork, fix, commit and send a pull request for the maintainers to review and merge into the main code base.

**Getting started with Git and GitHub**

 * [Setting up Git for Windows and connecting to GitHub](http://help.github.com/win-set-up-git/)
 * [Forking a GitHub repository](http://help.github.com/fork-a-repo/)
 * [The simple guide to GIT guide](http://rogerdudler.github.com/git-guide/)
 * [Open an issue](https://github.com/engineering87/MLDataContentionPrediction/issues) if you encounter a bug or have a suggestion for improvements/features

### Licensee
MLDataContentionPrediction source code is available under MIT License, see license in the source.

### Contact
Please contact at francesco.delre.87[at]gmail.com for any details.
