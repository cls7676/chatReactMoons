// Copyright (c) Microsoft. All rights reserved.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.SemanticKernel.Skills.MsGraph;

/// <summary>
/// Interface for email connections (e.g. Outlook).
/// </summary>
public interface IEmailConnector
{
    /// <summary>
    /// Get the user's email address.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user's email address.</returns>
    Task<string> GetMyEmailAddressAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Send an email to the s