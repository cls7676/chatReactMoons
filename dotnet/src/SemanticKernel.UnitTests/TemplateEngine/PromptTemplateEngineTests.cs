// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.TemplateEngine;
using Microsoft.SemanticKernel.TemplateEngine.Blocks;
using Moq;
using SemanticKernel.UnitTests.XunitHelpers;
using Xunit;

namespace SemanticKernel.UnitTests.TemplateEngine;

public sealed class PromptTemplateEngineTests
{
    private readonly IPromptTemplateEngine _target;
    private readonly ContextVariables _variables;
    private readonly Mock<IReadOnlySkillCollection> _skills;
    private readonly ILogger _logger;

    public PromptTemplateEngineTests()
    {
        this._logger = ConsoleLogger.Log;
        this._target = new PromptTemplateEngine(this._logger);
        this._variables = new ContextVariables(Guid.NewGuid().ToString("X"));
        this._skills = new Mock<IReadOnlySkillCollection>();
    }

    [Fact]
    public void ItTokenizesEdgeCases1()
    {
        // Arrange
        var template = "}}{{{ {$a}}}} {{b}}x}}";

        // Act
        var blocks = this._target.ExtractBlocks(template, false);

        // Assert
        Assert.Equal(5, blocks.Count);

        Asser