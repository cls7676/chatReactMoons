// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Moq;
using Xunit;

namespace SemanticKernel.UnitTests.Orchestration;

public sealed class SKFunctionTests2
{
    private readonly Mock<ILogger> _log;
    private readonly Mock<IReadOnlySkillCollection> _skills;

    private static string s_expected = string.Empty;
    private static string s_canary = string.Empty;

    public SKFunctionTests2()
    {
        this._log = new Mock<ILogger>();
        this._skills = new Mock<IReadOnlySkillCollection>();

        s_canary = "";
        s_expected = Guid.NewGuid().ToString("D");
    }

    [Fact]
    public async Task ItSupportsType1Async()
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        static void Test()
        {
            s_canary = s_expected;
        }

        var context = this.MockContex