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
            .GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod)
            .Where(m => m.Name != "GetType" && m.Name != "Equals" && m.Name != "GetHashCode" && m.Name != "ToString")
            .ToArray();

        IEnumerable<ISKFunction> functions = from method in methods select SKFunction.FromNativeMethod(method, skillInstance, "skill");
        List<ISKFunction> result = (from function in functions where function != null select function).ToList();

        // Act - Assert that no exception occurs and functions are not null
        Assert.Equal(26, methods.Length);
        Assert.Equal(26, result.Count);
        foreach (var method in methods)
        {
            ISKFunction? func = SKFunction.FromNativeMethod(method, skillInstance, "skill");
            Assert.NotNull(func);
        }
    }

    [Fact]
    public void ItThrowsForInvalidFunctions()
    {
        // Arrange
        var instance = new InvalidSkill();
        MethodInfo[] methods = instance.GetType()
            .GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod)
            .Where(m => m.Name != "GetType" && m.Name != "Equals" && m.Name != "GetHashCode")
            .ToArray();

        // Act - Assert that no exception occurs
        var count = 0;
        foreach (var method in methods)
        {
            try
            {
                SKFunction.FromNativeMethod(method, instance, "skill");
            }
            catch (KernelException e) when (e.ErrorCode == KernelException.ErrorCodes.FunctionTypeNotSupported)
            {
                count++;
            }
        }

        // Assert
        Assert.Equal(3, count);
    }

    private class InvalidSkill
    {
        [SKFunction("one")]
        public void Invalid1(string x, string y)
        {
        }

        [SKFunction("two")]
        public void Invalid2(SKContext cx, string y)
        {
        }

        [SKFunction("three")]
        public void Invalid3(string y, int n)
        {
        }
    }

    private class LocalExampleSkill
    {
        [SKFunction("one")]
        public void Type01()
       