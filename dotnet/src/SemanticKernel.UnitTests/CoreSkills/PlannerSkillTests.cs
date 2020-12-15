// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Orchestration.Extensions;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.SkillDefinition;
using Xunit;
using Xunit.Abstractions;

namespace SemanticKernel.UnitTests.CoreSkills;

public class PlannerSkillTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    private const string FunctionFlowRunnerText = @"
<goal>
Solve the equation x^2 = 2.
</goal>
<plan>
  <function.math.simplify input=""x^2 = 2"" />
</plan>
";

    private const string GoalText = "Solve the equation x^2 = 2.";

    public PlannerSkillTests(ITestOutputHelper testOutputHelper)
    {
        this._testOutputHelper = testOutputHelper;
        this._testOutputHelper.WriteLine("Tests initialized");
    }

    [Fact]
    public void ItCanBeInstantiated()
    {
        // Arrange
        var kernel = KernelBuilder.Create();
        _ = kernel.Config.AddOpenAICompletionBackend("test", "test", "test");

        // Act - Assert no exception occurs
        _ = new PlannerSkill(kernel);
    }

    [Fact]
    public void ItCanBeImported()
    {
        // Arrange
        var kernel = KernelBuilder.Create();
        _ = kernel.Config.AddOpenAICompletionBackend("test", "test", "test");

        // Act - Assert no exception occurs e.g. due to reflection
        _ = kernel.ImportSkill(new PlannerSkill(kernel), "planner");
    }

    [Fact]
    public async Task ItCanCreatePlanAsync()
    {
        // Arrange
        var kernel = KernelBuilder.Create();
        _ = kernel.Config.AddOpenAICompletionBackend("test", "test", "test");
        var plannerSkill = new PlannerSkill(kernel);
        var planner = kernel.ImportSkill(plannerSkill, "planner");

        // Act
        var context = await kernel.RunAsync(GoalText, planner["CreatePlan"]);

        // Assert
        var plan = context.Variables.ToPlan();
        Assert.NotNull(plan);
        Assert.NotNull(plan.Id);
        Assert.Equal(GoalText, plan.Goal);
        Assert.StartsWith("<goal>\nSolve the equation x^2 = 2.\n</goal>", plan.PlanString, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ItCanExecutePlanTextAsync()
    {
        // Arrange
        var kernel = KernelBuilder.Create();
        _ = kernel.Config.AddOpenAICompletionBackend("test", "test", "test");
        var plannerSkill = kernel.ImportSkill(new PlannerSkill(kernel));

        // Act
        var context = await kernel.RunAsync(FunctionFlowRunnerText, plannerSkill["ExecutePlan"]);

        // Assert
        var plan = context.Variables.ToPlan();
        Assert.NotNull(plan);
        Assert.NotNull(plan.Id);

        // Since not using Plan or PlanExecution object, this won't be present.
        // Maybe we do work to parse this out. Not doing too much though since we might move to json instead of xml.
        // Assert.Equal(GoalText, plan.Goal);
    }

    [Fact]
    public async Task ItCanExecutePlanAsync()
    {
        // Arrange
        var kernel = KernelBuilder.Create();
        _ = kernel.Config.AddOpenAICompletionBackend("test", "test", "test");
        var plannerSkill = kernel.ImportSkill(new PlannerSkill(kernel));
        Plan createdPlan = new()
        {
            Goal = GoalText,
            PlanString = FunctionFlowRunnerText
        };

        // Act
        var variables = new ContextVariables();
        _ = variables.UpdateWithPlanEntry(createdPlan);
        var context = await kernel.RunAsync(variables, plannerSkill["ExecutePlan"]);

        // Assert
        var plan = context.Variables.ToPlan();
        Assert.NotNull(plan);
        Assert.NotNull(plan.Id);
        Assert.Equal(GoalText, plan.Goal);
    }

    [Fact]
    public async Task ItCanCreateSkillPlanAsync()
    {
        // Arrange
        var kernel = KernelBuilder.Create();
        _ = kernel.Config.AddOpenAICompletionBackend("test", "test", "test");
        var plannerSkill = kernel.ImportSkill(new PlannerSkill(kernel));

        // Act
        var context = await kernel.RunAsync(GoalText, plannerSkill["CreatePlan"]);

        // Assert
        var plan = context.Variables.ToPlan();
        Assert.NotNull(plan);
        Assert.NotNull(plan.Id);
        Assert.Equal(GoalText, plan.Goal);
        Assert.StartsWith("<goal>\nSolve the equation x^2 = 2.\n</goal>", plan.PlanString, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ItCanExecutePlanJsonAsync()
    {
        // Arrange
        var kernel = KernelBuilder.Create();
        _ = kernel.Config.AddOpenAICompletionBackend("test", "test", "test");
        var plannerSkill = kernel.ImportSkill(new PlannerSkill(kernel));
        Plan createdPlan = new()
        {
            Goal = GoalText,
            PlanString = FunctionFlowRunnerText
        };

        // Act
        var context = await kernel.RunAsync(createdPlan.ToJson(), plannerSkill["ExecutePlan"]);

        // Assert
        var plan = context.Variables.ToPlan();
        Assert.NotNull(plan);
        Assert.NotNull(plan.Id);
        Assert.Equal(GoalText, plan.Goal);
    }

    [Fact]
    public async Task NoGoalExecutePlanReturnsInvalidResultAsync()
    {
        // Arrange
        var kernel = KernelBuilder.Create();
        _ = kernel.Config.AddOpenAICompletionBackend("test", "test", "test");
        var plannerSkill = kernel.ImportSkill(new PlannerSkill(kernel));

        // Act
        var context = await kernel.RunAsync(GoalText, plannerSkill["ExecutePlan"]);

        // Assert
        var plan = context.Variables.ToPlan();
        Assert.NotNull(plan);
        Assert.NotNull(plan.Id);
        Assert.Equal(string.Empty, plan.Goal);
        Assert.Equal(GoalText, plan.PlanString);
        Assert.False(plan.IsSuccessful);
        Assert.True(plan.IsComplete);
        Assert.Contains("No goal found.", plan.Result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task InvalidPlanExecutePlanReturnsInvalidResultAsync()
    {
        // Arrange
        var kernel = KernelBuilder.Create();
        _ = kernel.Config.AddOpenAICompletionBackend("test", "test", "test");
        var plannerSkill = kernel.ImportSkill(new PlannerSkill(kernel));

        // Act
        var context = await kernel.RunAsync("<someTag>" + GoalText, plannerSkill["ExecutePlan"]);

        // Assert
        var plan = context.Variables.ToPlan();
        Assert.NotNull(plan);
        Assert.NotNull(plan.Id);
        Assert.Equal(string.Empty, plan.Goal);
        Assert.Equal("<someTag>" + GoalText, plan.PlanString);
        Assert.False(plan.IsSuccessful);
        Assert.True(plan.IsComplete);
        Assert.Contains("Failed to parse plan xml.", plan.Result, StringComparison.OrdinalIgnoreCase);
    }

    //
    // Advanced tests for ExecutePlan that use mock sk functions to test the flow
    //
    [Theory]
    [InlineData("Test the functionFlowRunner", @"<goal>Test the functionFlowRunner</goal>
<plan>
<function.MockSkill.Echo input=""Hello World"" />
</plan>")]
    public async Task ExecutePlanCanCallFunctionAsync(string goalText, string planText)
    {
        // Arrange
        var kernel = KernelBuilder.Create();
        _ = kernel.Config.AddOpenAICompletionBackend("test", "test", "test");
        var plannerSkill = kernel.ImportSkill(new PlannerSkill(kernel));
        _ = kernel.ImportSkill(new MockSkill(this._testOutputHelper), "MockSkill");
        Plan createdPlan = new()
        {
            Goal = goalText,
            PlanString = planText
        };

        // Act
        var context = await kernel.RunAsync(createdPlan.ToJson(), plannerSkill["ExecutePlan"]);

        // Assert
        var plan = context.Variables.ToPlan();
        Assert.NotNull(plan);
        Assert.NotNull(plan.Id);
        Assert.Equal(goalText, plan.Goal);
        Assert.True(plan.IsSuccessful);
        Assert.True(plan.IsComplete);
        Assert.Equal("Echo Result: Hello World", plan.Result, true);
    }

    // Test that contains a #text node in the plan
    [Theory]
    [InlineData("Test the functionFlowRunner", @"<goal>Test the functionFlowRunner</goal>
<plan>
<function.MockSkill.Echo input=""Hello World"" />
This is some text
</plan>")]
    public async Task ExecutePlanCanCallFunctionWithTextAsync(string goalText, string planText)
    {
        // Arrange
        var kernel = KernelBuilder.Create();
        _ = kernel.Config.AddOpenAICompletionBackend("test", "test", "test");
        var plannerSkill = kernel.ImportS