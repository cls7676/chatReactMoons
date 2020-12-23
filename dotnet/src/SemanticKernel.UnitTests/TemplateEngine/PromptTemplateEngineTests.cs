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

        Assert.Equal("}}{", blocks[0].Content);
        Assert.Equal(BlockTypes.Text, blocks[0].Type);

        Assert.Equal("{$a", blocks[1].Content);
        Assert.Equal(BlockTypes.Code, blocks[1].Type);

        Assert.Equal("}} ", blocks[2].Content);
        Assert.Equal(BlockTypes.Text, blocks[2].Type);

        Assert.Equal("b", blocks[3].Content);
        Assert.Equal(BlockTypes.Code, blocks[3].Type);

        Assert.Equal("x}}", blocks[4].Content);
        Assert.Equal(BlockTypes.Text, blocks[4].Type);
    }

    [Fact]
    public void ItTokenizesEdgeCases2()
    {
        // Arrange
        var template = "}}{{{{$a}}}} {{b}}$x}}";

        // Act
        var blocks = this._target.ExtractBlocks(template);

        // Assert
        Assert.Equal(5, blocks.Count);

        Assert.Equal("}}{{", blocks[0].Content);
        Assert.Equal(BlockTypes.Text, blocks[0].Type);

        Assert.Equal("$a", blocks[1].Content);
        Assert.Equal(BlockTypes.Variable, blocks[1].Type);

        Assert.Equal("}} ", blocks[2].Content);
        Assert.Equal(BlockTypes.Text, blocks[2].Type);

        Assert.Equal("b", blocks[3].Content);
        Assert.Equal(BlockTypes.Code, blocks[3].Type);

        Assert.Equal("$x}}", blocks[4].Content);
        Assert.Equal(BlockTypes.Text, blocks[4].Type);
    }

    [Fact]
    public void ItTokenizesAClassicPrompt()
    {
        // Arrange
        var template = "this is a {{ $prompt }} with {{$some}} variables " +
                       "and {{function $calls}} that {{ also $use $variables }}";

        // Act
        var blocks = this._target.ExtractBlocks(template, true);

        // Assert
        Assert.Equal(8, blocks.Count);

        Assert.Equal("this is a ", blocks[0].Content);
        Assert.Equal(BlockTypes.Text, blocks[0].Type);

        Assert.Equal("$prompt", blocks[1].Content);
        Assert.Equal(BlockTypes.Variable, blocks[1].Type);

        Assert.Equal(" with ", blocks[2].Content);
        Assert.Equal(BlockTypes.Text, blocks[2].Type);

        Assert.Equal("$some", blocks[3].Content);
        Assert.Equal(BlockTypes.Variable, blocks[3].Type);

        Assert.Equal(" variables and ", blocks[4].Content);
        Assert.Equal(BlockTypes.Text, blocks[4].Type);

        Assert.Equal("function $calls", blocks[5].Co