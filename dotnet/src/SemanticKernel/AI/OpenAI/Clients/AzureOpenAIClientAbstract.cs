
﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.AI.OpenAI.HttpSchema;
using Microsoft.SemanticKernel.Text;

namespace Microsoft.SemanticKernel.AI.OpenAI.Clients;

/// <summary>
/// An abstract Azure OpenAI Client.
/// </summary>
public abstract class AzureOpenAIClientAbstract : OpenAIClientAbstract
{
    /// <summary>
    /// Default Azure OpenAI REST API version
    /// </summary>
    protected const string DefaultAzureAPIVersion = "2022-12-01";

    /// <summary>
    /// Azure OpenAI API version
    /// </summary>
    protected string AzureOpenAIApiVersion
    {
        get { return this._azureOpenAIApiVersion; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new AIException(
                    AIException.ErrorCodes.InvalidConfiguration,
                    "Invalid Azure OpenAI API version, the value is empty");
            }

            this._azureOpenAIApiVersion = value;
        }
    }

    /// <summary>
    /// Azure endpoint of your models
    /// </summary>
    protected string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Construct an AzureOpenAIClientAbstract object
    /// </summary>
    /// <param name="log">Logger</param>
    protected AzureOpenAIClientAbstract(ILogger? log = null) : base(log)
    {
    }

    /// <summary>
    /// Returns the deployment name of the model ID
    /// </summary>
    /// <param name="modelId">Azure OpenAI Model ID</param>
    /// <returns>Name of the deployment for the model ID</returns>
    /// <exception cref="AIException">AIException thrown during request.</exception>
    protected async Task<string> GetDeploymentNameAsync(string modelId)
    {
        string fullModelId = this.Endpoint + ":" + modelId;

        // If the value is a deployment name
        if (s_deploymentToModel.ContainsKey(fullModelId))
        {
            return modelId;
        }

        // If the value is a model ID present in the cache
        if (s_modelToDeployment.TryGetValue(fullModelId, out string modelIdCached))
        {
            return modelIdCached;
        }

        // If the cache has already been warmed up
        string modelsAvailable;
        if (s_deploymentsCached.ContainsKey(this.Endpoint))
        {
            modelsAvailable = string.Join(", ", s_modelToDeployment.Keys);
            throw new AIException(
                AIException.ErrorCodes.ModelNotAvailable,
                $"Model '{modelId}' not available on {this.Endpoint}. " +
                $"Available models: {modelsAvailable}. Deploy the model and restart the application.");
        }

        await this.CacheDeploymentsAsync();

        if (s_modelToDeployment.TryGetValue(fullModelId, out string modelIdAfterCache))
        {
            return modelIdAfterCache;
        }

        modelsAvailable = string.Join(", ", s_modelToDeployment.Keys);
        throw new AIException(
            AIException.ErrorCodes.ModelNotAvailable,
            $"Model '{modelId}' not available on {this.Endpoint}. " +
            $"Available models: {modelsAvailable}. Deploy the model and restart the application.");
    }

    /// <summary>
    /// Caches the list of deployments in Azure OpenAI.
    /// </summary>
    /// <returns>An async task</returns>
    /// <exception cref="AIException">AIException thrown during the request.</exception>