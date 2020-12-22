// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Xunit;

namespace SemanticKernel.UnitTests.Orchestration;

public sealed class SKFunctionTests3
{
    [Fact]
    public void ItDoesntThrowForValidFunctions()
    {
        // Arrange
        var skillInstance = new LocalExampleSkill();
        MethodInfo[] methods = skillInstance.GetType()
            .GetMethods(BindingFlags.Stati