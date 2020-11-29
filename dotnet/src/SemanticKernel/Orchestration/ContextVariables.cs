// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.SemanticKernel.Diagnostics;

namespace Microsoft.SemanticKernel.Orchestration;

/// <summary>
/// Context Variables is a data structure that holds temporary data while a task is being performed.
/// It is accessed and manipulated by functions in the pipeline.
/// </summary>
public sealed class ContextVariables : IEnumerable<KeyValuePair<string, string>>
{
    /// <summary>
    /// In the simplest scenario, the data is an input string, stored here.
    /// </summary>
    public string Input => this._variables[MainKey];

    /// <summary>
    /// Constructor for context variables.
    /// </summary>
    /// <param name="content">Optional value for the main variable of the context.</param>
    public ContextVariables(string content = "")
    {
        this._variables[MainKey] = content;
    }

    /// <summary>
    /// Updates the main input text with the new value after a function is complete.
    /// </summary>
    /// <param name="content">The new input value, for the ne