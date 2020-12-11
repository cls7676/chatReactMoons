// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace SemanticKernel.Skills.UnitTests;

/// <summary>
/// A logger that writes to the Xunit test output
/// </summary>
internal sealed class XunitLogger<T> : ILogger<T>, IDisposable
{
    private readonly ITestOutputHelper _output;

    public XunitLogger(ITestOutputHelper output)
    {
        this._output = output;
    }

    /// <inheritdoc/>
    public void Lo