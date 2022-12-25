# Semantic Kernel Service API (For Learning Samples)

Watch the [Service API Quick Start Video](https://aka.ms/SK-Local-API-Setup).

This service API is written in C# against Azure Function Runtime v4 and exposes
some Semantic Kernel APIs that you can call via HTTP POST requests for the learning samples.

![azure-function-diagram](https://user-images.githubusercontent.com/146438/222305329-0557414d-38ce-4712-a7c1-4f6c63c20320.png)


**!IMPORTANT**

> This service API is for educational purposes only and should not be used in any production use
> case. It is intended to highlight concepts of Semantic Kernel and not any architectural
> security design practices to be used.

## Prerequisites

[Azure Functions Core Tools](https://learn.microsoft.com/azure/azure-functions/functions-run-local)
installation is required for this service API to run locally.

## Running the service API locally

**Run** `func start --csharp` from the command line. This will run the service API locally at `http://localhost:7071`.

Two endpoints will be exposed by the service API:

-   **InvokeFunction**: [POST] `http://localhost:7071/api/skills/{skillName}/invoke/{