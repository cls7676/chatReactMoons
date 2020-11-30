// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.SemanticKernel.Diagnostics;

namespace Microsoft.SemanticKernel.Planning;

/// <summary>
/// Planning exception.
/// </summary>
public class PlanningException : Exception<PlanningException.ErrorCodes>
{
    /// <summary>
    /// Error codes for <see cref="PlanningException"/>.
    /// </summary>
    public enum ErrorCodes
    {
        /// <summary>
        /// Unknown error.
        /// </summary>
        UnknownError = -1,

        /// <summary>
        /// Invalid plan.
        /// </summary>
        InvalidPlan = 0,

        /// <summary>
        /// In