// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.AI.OpenAI.Clients;
using Microsoft.SemanticKernel.AI.OpenAI.HttpSchema;
using Microsoft.SemanticKernel.Diagnostics;
using Microsoft.SemanticKernel.Text;

namespace Microsoft.SemanticKernel.AI.OpenAI.Services;

/// <summary>
/// Azure OpenAI text embedding service.
/// </summary>
public sealed class AzureTextEmbeddings : AzureOpenAIClientAbstract, IEmbeddingGenerator<string, float>
{
    private readonly string _modelId;

    /// <summary>
    /// Creates a new AzureTextEmbeddings client instance
    /// </summary>
    /// <param name="modelId">Azure OpenAI model ID or deployment name, see https://learn.microsoft.com/azure/cognitive-services/openai/how-to/create-resource</param>
    /// <param name="endpoint">Azure OpenAI deplo