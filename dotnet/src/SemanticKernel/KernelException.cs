// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.SemanticKernel.Diagnostics;

namespace Microsoft.SemanticKernel;

/// <summary>
/// Kernel logic exception
/// </summary>
public class KernelException : Exception<KernelException.ErrorCodes>
{
    /// <summary>
    /// Semantic kernel error codes.
    /// </summary>
    public enum ErrorCodes
    {
        /// <summary>
        /// Unknown error.
        /// </summary>
        Unk