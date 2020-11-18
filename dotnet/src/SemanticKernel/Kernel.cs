﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.OpenAI.Services;
using Microsoft.SemanticKernel.Configuration;
using Microsoft.SemanticKernel.Diagnostics;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.TemplateEngine;
using Microsoft.SemanticKernel.Text;

namespace Microsoft.SemanticKernel;

/// <summary>
/// Semantic kernel class.
/// The kernel provides a skill collection to define native and semantic functions, an orchestrator to execute a list of functions.
/// Semantic functions are automatically rendered and executed using an internal prompt template rendering engine.
/// Future versions will allow to:
/// * customize the rendering engine
/// * include branching logic in the functions pipeline
/// * persist execution state for long running pipelines
/// * distribute pipelines over a network
/// * RPC functions and secure environments, e.g. sandboxing and credentials management
/// * auto-generate pipelines given a higher level goal
/// </summary>
public sealed class Kernel : IKernel, IDisposable
{
    /// <inheritdoc/>
    public KernelConfig Config => this._config;

    /// <inheritdoc/>
    public ILogger Log => this._log;

    /// <inheritdoc/>
    public ISemanticTextMemory Memory => this._memory;

    /// <inheritdoc/>
    public IReadOnlySkillCollection Skills => this._skillCollection.ReadOnlySkillCollection;

    /// <inheritdoc/>
    public IPromptTemplateEngine PromptTemplateEngine => this._promptTemplateEngine;

    /// <summary>
    /// Return a new instance of the kernel builder, used to build and configure kernel instances.
    /// </summary>
    public static KernelBuilder Builder => new();

    /// <summary>
    /// Kernel constructor. See KernelBuilder for an easier and less error prone approach to create kernel instances.
    /// </summary>
    /// <param name="skillCollection"></param>
    /// <param name="promptTemplateEngine"></param>
    /// <param name="memory"></param>
    /// <param name="config"></param>
    /// <param name="log"></param>
    public Kernel(
        ISkillCollection skillCollection,
        IPromptTemplateEngine promptTemplateEngine,
        ISemanticTextMemory memory,
        KernelConfig config,
        ILogger log)
    {
        this._log = log;
        this._config = config;
        this._memory = memory;
        this._promptTemplateEngine = promptTemplateEngine;
        this._skillCollection = skillCollection;
    }

    /// <inheritdoc/>
    public ISKFunction RegisterSemanticFunction(string functionName, SemanticFunctionConfig functionConfig)
    {
        return this.RegisterSemanticFunction(SkillCollection.GlobalSkill, functionName, functionConfig);
    }

    /// <inheritdoc/>
    public ISKFunction RegisterSemanticFunction(string skillName, string functionName, SemanticFunctionConfig functionConfig)
    {
        // Future-proofing the name not to contain special chars
        Verify.ValidSkillName(skillName);
        Verify.ValidFunctionName(functionName);

        ISKFunction function = this.CreateSemanticFunction(skillName, functionName, functionConfig);
        this._skillCollection.AddSemanticFunction(function);

        return function;
    }

    /// <inheritdoc/>
    public IDictionary<string, ISKFunction> ImportSkill(object skillInstance, string skillName = "")
    {
        if (string.IsNullOrWhiteSpace(skillName))
        {
            skillName = SkillCollection.GlobalSkill;
            this._log.LogTrace("Importing skill {0} in the global namespace", skillInstance.GetType().FullName);
        }
        else
        {
            this._log.LogTrace("Importing skill {0}", skillName);
        }

        var skill = new Dictionary<string, ISKFunction>(StringComparer.OrdinalIgnoreCase);
        IEnumerable<ISKFunction> functions = ImportSkill(skillInstance, skillName, this._log);
        foreach (ISKFunction f in functions)
        {
            f.SetDefaultSkillCollection(this.Skills);
            this._skillCollection.AddNativeFunction(f);
            skill.Add(f.Name, f);
        }

        return skill;
    }

    /// <inheritdoc/>
    public void RegisterMemory(ISemanticTextMemory memory)
    {
        this._memory = memory;
    }

    /// <inheritdoc/>
    public Task<SKContext> RunAsync(params ISKFunction[] pipeline)
        => this.RunAsync(new ContextVariables(), pipeline);

    /// <inheritdoc/>
    public Task<SKContext> RunAsync(string input, params ISKFunction[] pipeline)
        => this.RunAsync(new ContextVariables(input), pipeline);

    /// <inheritdoc/>
    public Task<SKContext> RunAsync(ContextVariables variables, params ISKFunction[] pipeline)
        => this.RunAsync(variables, CancellationToken.None, pipeline);

    /// <inheritdoc/>
    public Task<SKContext> RunAsync(CancellationToken cancellationToken, params ISKFunction[] pipeline)
        => this.RunAsync(new ContextVariables(), cancellationToken, pipeline);

    /// <inheritdoc/>
    public Task<SKContext> RunAsync(string input, CancellationToken cancellationToken, params ISKFunction[] pipeline)
        => this.RunAsync(new ContextVariables(input), cancellationToken, pipeline);

    /// <inheritdoc/>
    public async Task<SKContext> RunAsync(ContextVariables variables, CancellationToken cancellationToken, params ISKFunction[] pipeline)
    {
        var context = new SKContext(
            variables,
            this._memory,
            this._skillCollection.ReadOnlySkillCollection,
            this._log,
            cancellationToken);

        int pipelineStepCount = -1;
        foreach (ISKFunction f in pipeline)
        {
            if (context.ErrorOccurred)
            {
                this._log.LogError(
                    context.LastException,
                    "Something went wrong in pipeline step {0}:'{1}'", pipelineStepCount, context.LastErrorDescription);
                return context;
            }

            pipelineStepCount++;

            try
            {
                cancellationToken.ThrowIfCancellati