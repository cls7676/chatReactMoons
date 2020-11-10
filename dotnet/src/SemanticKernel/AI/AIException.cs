// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.SemanticKernel.Diagnostics;

namespace Microsoft.SemanticKernel.AI;

/// <summary>
/// AI logic exception
/// </summary>
public class AIException : Exception<AIException.ErrorCodes>
{
    /// <summary>
    /// Possible error codes for exceptions
    /// </summary>
    public enum ErrorCodes
    {
        /// <summary>
        /// Unknown error.
        /// </summary>
        UnknownError = -1,

        /// <summary>
        /// No response.
        /// </summary>
        NoResponse,

        /// <summary>
        /// Access is denied.
        /// </summary>
        AccessDenied,

        /// <summary>
        /// The request was invalid.
        /// </summary