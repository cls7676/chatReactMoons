// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using KernelHttpServer.Config;
using KernelHttpServer.Utils;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using static KernelHttpServer.Config.Constants;

namespace KernelHttpServer;

internal static class SemanticKernelFactory
{
    internal static IKernel? CreateForRequest(
        HttpRequestData req,
        ILogger logger,
        IEnumerable<string>? skillsToLoad = null,
        IMemoryStore<float>? memoryStore = null)
    {
        var apiConfig = req.ToApiKeyConfig();

        //must have a completion backend
        if (!apiConfig.CompletionConfig.IsValid())
        {
            logger.LogError("Completion backend has not been supplied.");
            return null;
        }

        //embedding backend is optional, don't fail if we were not given the config
        if (memoryStore != null &&
            !apiConfig.EmbeddingConfig.IsValid())
        {
            logger.LogWarning("Embedding backend has not been supplied.");
        }

        KernelBuilder builder = Kernel.Builder;
        builder =