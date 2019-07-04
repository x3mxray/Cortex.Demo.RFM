# Sitecore Cortex and Machine Learning Demo
Segmentation and clustering contacts by RFM values.
Example of usage Sitecore Cortex with ML.Net.

Contains 2 solutions:
- [DemoCortex](https://github.com/x3mxray/Cortex.Demo.RFM/tree/master/DemoCortex)
- [MLServer](https://github.com/x3mxray/Cortex.Demo.RFM/tree/master/MLServer)

Full overview of demo located [here](https://www.brimit.com/blog/dive-sitecore-cortex-machine-learning-introduction)

# How to Deploy #

## Sitecore
Install the sitecore package [RFM Demo-1.0 (sitecore package).zip](https://github.com/x3mxray/Cortex.Demo.RFM/blob/master/install/RFM_Demo-1.0_(sitecore_package).zip)

## xConnect and jobs
* Extract and copy [xconnect](https://github.com/x3mxray/Cortex.Demo.RFM/tree/master/install/xconnect.zip) to your xConnect instance
* Restart xConnect instance and xconnect jobs in windows services

# How to populate xConnect with testing data #
* Build solution.
* Run Demo.Project.DemoDataExplorer.exe from project Demo.Project.DemoDataExplorer.
![Data Explorer](https://github.com/x3mxray/Cortex.Demo.RFM/blob/master/documentation/images/DataExplorer.jpg)
* Copy your sitecore website root url to "API address"
* Click "Browse" and select [Online Retail.xlsx ~500k records](https://github.com/x3mxray/Cortex.Demo.RFM/blob/master/install/Online_Retail.xlsx)
* Click "Upload file". Wait for finishing uploading process (in takes ~10-15 min). During process you can see logs in sitecore instance and new contacts appearance in Experience Profile:
```
INFO  Excel import: 272 from 4339: CustomerID=15332
```

# How to run ML server #
* Run [MLServer solution](https://github.com/x3mxray/Cortex.Demo.RFM/tree/master/MLServer) in IIS Express (or install it as IIS application)
* Make sure that it is accessible by requesting http://localhost:56399/api/rfm/test
* If you change localhost url to your own, make corresponding change in [Processing Engine -> sc.Processing.Services.MLNet.xml](https://github.com/x3mxray/Cortex.Demo.RFM/blob/master/install/xconnect/App_Data/jobs/continuous/ProcessingEngine/App_Data/Config/Sitecore/Demo/sc.Processing.Services.MLNet.xml)

# How to run Cortex tasks #
- POST request to http://sitecoreInstance.url/api/contactapi/RegisterTasks with POSTMAN
- Or change processing agent sleep period in [Processing Engine -> sc.Processing.Engine.DemoAgents.xml](https://github.com/x3mxray/Cortex.Demo.RFM/blob/master/install/xconnect/App_Data/jobs/continuous/ProcessingEngine/App_Data/Config/Sitecore/Demo/sc.Processing.Engine.DemoAgents.xml)
- All processes execution takes ~10-15 minutes (~500k records). During process you can see logs in Processing Engine job:
```
[Information] Registered Distributed Processing Task, TaskId: 19260a83-e180-457a-9bdb-b9210f6e757f, Worker: Sitecore.Processing.Engine.ML.Workers.ProjectionWorker`1[[Sitecore.XConnect.Interaction, Sitecore.XConnect, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null]], Sitecore.Processing.Engine.ML, DataSource: Sitecore.Processing.Engine.DataSources.DataExtraction.InteractionDataSource, Sitecore.Processing.Engine
[Information] Registered Deferred Processing Task, Id: 1a0f5ca3-2118-4c43-a57d-2dcabce48a16, Worker: Sitecore.Processing.Engine.ML.Workers.MergeWorker, Sitecore.Processing.Engine.ML
[Information] Registered Deferred Processing Task, Id: 4eb84501-15f2-4861-849d-cb671d932dfd, Worker: Demo.Foundation.ProcessingEngine.Train.Workers.RfmTrainingWorker, Demo.Foundation.ProcessingEngine
[Information] Registered Distributed Processing Task, TaskId: b8784a8a-ac13-4d86-af86-676a6fe11bc1, Worker: Demo.Foundation.ProcessingEngine.Predict.Workers.RfmEvaluationWorker, Demo.Foundation.ProcessingEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null, DataSource: Sitecore.Processing.Engine.DataSources.DataExtraction.ContactDataSource, Sitecore.Processing.Engine
[Information] TaskAgent Executing worker. Machine: BRIMIT-SBA-PC, Process: 26164, AgentId: 8, TaskId: 19260a83-e180-457a-9bdb-b9210f6e757f, TaskType: DistributedProcessing.
[Information] TaskAgent Executing worker. Machine: BRIMIT-SBA-PC, Process: 26164, AgentId: 7, TaskId: 1a0f5ca3-2118-4c43-a57d-2dcabce48a16, TaskType: DeferredAction.
[Information] TaskAgent Worker execution completed. Machine: BRIMIT-SBA-PC, Process: 26164, AgentId: 8, TaskId: 19260a83-e180-457a-9bdb-b9210f6e757f, TaskType: DistributedProcessing.
[Information] TaskAgent Worker execution completed. Machine: BRIMIT-SBA-PC, Process: 26164, AgentId: 7, TaskId: 1a0f5ca3-2118-4c43-a57d-2dcabce48a16, TaskType: DeferredAction.
[Information] TaskAgent Executing worker. Machine: BRIMIT-SBA-PC, Process: 26164, AgentId: 7, TaskId: 4eb84501-15f2-4861-849d-cb671d932dfd, TaskType: DeferredAction.
[Information] RfmTrainingWorker.RunAsync
[Information] Update RFM info: customerId=12534, R=2, F=2, M=2, Recency=1, Frequency=63, Monetary=1089.18
[Information] Update RFM info: customerId=14947, R=2, F=1, M=1, Recency=59.2618055555556, Frequency=14, Monetary=290.82
[Information] Update RFM info: customerId=17941, R=2, F=1, M=1, Recency=1, Frequency=2, Monetary=304.56
...
[Information] TaskAgent Worker execution completed. Machine: BRIMIT-SBA-PC, Process: 26164, AgentId: 7, TaskId: 4eb84501-15f2-4861-849d-cb671d932dfd, TaskType: DeferredAction.
[Information] TaskAgent Executing worker. Machine: BRIMIT-SBA-PC, Process: 26164, AgentId: 7, TaskId: b8784a8a-ac13-4d86-af86-676a6fe11bc1, TaskType: DistributedProcessing.
[Information] RFM info: email=demo23296713e0fc4a9c83e756733a0c35d4@gmail.com, R=1, F=1, M=1, Recency=1, Frequency=12, Monetary=227.39, CLUSTER=5
[Information] RFM info: email=demo24446a92650d4f91816352197c0aba3b@gmail.com, R=1, F=1, M=1, Recency=1, Frequency=12, Monetary=196.89, CLUSTER=5
[Information] RFM info: email=demo107c92a74a8a43359672efc7243d0e2b@gmail.com, R=1, F=3, M=2, Recency=1, Frequency=77, Monetary=469.48, CLUSTER=4
...
[Information] RFM info: email=demo113822270ea6464e93b69607c4606a11@gmail.com, R=3, F=3, M=3, Recency=343.147222222222, Frequency=172, Monetary=3237.54, CLUSTER=2
[Information] TaskAgent Worker execution completed. Machine: BRIMIT-SBA-PC, Process: 26164, AgentId: 7, TaskId: b8784a8a-ac13-4d86-af86-676a6fe11bc1, TaskType: DistributedProcessing.
```

# Feedback #
If you are faced with any issues or have questions/suggestions you can contact me in sitecore slack channel [#cortexmachinelearning](https://sitecorechat.slack.com/messages/CD0BU3QBV/) @x3mxray