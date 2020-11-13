// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel.Diagnostics;

namespace Microsoft.SemanticKernel.AI.OpenAI.Services;

/// <summary>
/// Azure OpenAI configuration.
/// </summary>
public sealed class AzureOpenAIConfig : BackendConfig
{
    /// <summary>
    /// Azure OpenAI deployment name, see https://learn.microsoft.com/azure/cognitive-services/openai/how-to/create-resource
    /// </summary>
    public string DeploymentName { get; set; }

    /// <summary>
    /// Azure OpenAI deployment URL, see https://learn.mi