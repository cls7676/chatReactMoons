﻿// Copyright (c) Microsoft. All rights reserved.

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

        var context = this.MockContext("");

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(1);
        Assert.Equal(s_expected, s_canary);
    }

    [Fact]
    public async Task ItSupportsType2Async()
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        static string Test()
        {
            s_canary = s_expected;
            return s_expected;
        }

        var context = this.MockContext("");

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(2);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(s_expected, result.Result);
        Assert.Equal(s_expected, context.Result);
    }

    [Fact]
    public async Task ItSupportsType3Async()
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        static Task<string> Test()
        {
            s_canary = s_expected;
            return Task.FromResult(s_expected);
        }

        var context = this.MockContext("");

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(3);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(s_expected, context.Result);
        Assert.Equal(s_expected, result.Result);
    }

    [Fact]
    public async Task ItSupportsType4Async()
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        static void Test(SKContext cx)
        {
            s_canary = s_expected;
            cx["canary"] = s_expected;
        }

        var context = this.MockContext("xy");
        context["someVar"] = "qz";

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(4);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(s_expected, context["canary"]);
    }

    [Fact]
    public async Task ItSupportsType5Async()
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        static string Test(SKContext cx)
        {
            s_canary = cx["someVar"];
            return "abc";
        }

        var context = this.MockContext("");
        context["someVar"] = s_expected;

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(5);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal("abc", context.Result);
    }

    [Fact]
    public async Task ItSupportsType5NullableAsync()
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        string? Test(SKContext cx)
        {
            s_canary = cx["someVar"];
            return "abc";
        }

        var context = this.MockContext("");
        context["someVar"] = s_expected;

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(5);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal("abc", context.Result);
    }

    [Fact]
    public async Task ItSupportsType6Async()
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        Task<string> Test(SKContext cx)
        {
            s_canary = s_expected;
            cx.Variables["canary"] = s_expected;
            return Task.FromResult(s_expected);
        }

        var context = this.MockContext("");

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(6);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(s_canary, context.Result);
        Assert.Equal(s_expected, context["canary"]);
    }

    [Fact]
    public async Task ItSupportsType7Async()
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        Task<SKContext> Test(SKContext cx)
        {
            s_canary = s_expected;
            cx.Variables.Update("foo");
            cx["canary"] = s_expected;
            return Task.FromResult(cx);
        }

        var context = this.MockContext("");

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(7);
        Assert.Equal(s_expected, s_canary);
        Assert.Equal(s_expected, context["canary"]);
        Assert.Equal("foo", context.Result);
    }

    [Fact]
    public async Task ItSupportsType8Async()
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test")]
        void Test(string input)
        {
            s_canary = s_expected + input;
        }

        var context = this.MockContext(".blah");

        // Act
        var function = SKFunction.FromNativeMethod(Method(Test), log: this._log.Object);
        Assert.NotNull(function);
        SKContext result = await function.InvokeAsync(context);

        // Assert
        Assert.False(result.ErrorOccurred);
        this.VerifyFunctionTypeMatch(8);
        Assert.Equal(s_expected + ".blah", s_canary);
    }

    [Fact]
    public async Task ItSupportsType9Async()
    {
        // Arrange
        [SKFunction("Test")]
        [SKFunctionName("Test"