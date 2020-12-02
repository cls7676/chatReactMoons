// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.SemanticKernel.Diagnostics;

namespace Microsoft.SemanticKernel.TemplateEngine;

/// <summary>
/// Template exception.
/// </summary>
public class TemplateException : Exception<TemplateException.ErrorCodes>
{
    /// <summary>
    /// Error codes for <see cref="TemplateException"/>.
    /// </summary>
    public enum Erro