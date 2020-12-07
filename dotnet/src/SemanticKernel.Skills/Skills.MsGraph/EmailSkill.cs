// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Skills.MsGraph.Diagnostics;

namespace Microsoft.SemanticKernel.Skills.MsGraph;

/// <summary>
/// Email skill (e.g. Outlook).
/// </summary>
public class EmailSkill
{
    /// <summary>
    /// <see cref="ContextVariables"/> parameter names.
    /// </summary>
    public static class Parameters
    {
        /// <summary>
        /// Email recipients, separated by ',' or ';'.
        /// </summary>
        public const string Recipients = "recipients";

        /// <summary>
        /// Email subject.
        /// </summary>
        public const string Subject = "subject";
    }

    private readonly IEmailConnector _connector;
    private readonly ILogger<EmailSkill> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailSkill"/> class.
    /// </summary>
    /// <param name="connector">Email connector.</param>
    /// <param name="logger">Logger.</param>
    public EmailSkill(IEmailConnector connector, ILogger<EmailSkill>? logger = null)
    {
        Ensure.NotNull(connector, nameof(connector));

        this._connector = connector;
        this._logger = logger ?? new NullLogger<EmailSkill>();
    }

    /// <summary>
    /// Get my email address.
    /// </summary>
    [SKFunction("Gets the email address for me.")]
    public async Task<string> GetMyEmailAddressAsync()
        => await this._connector.GetMyEmailAddressAsync();

    /// <summary>
    /// Send an email using <see cref="ContextVariables.Input"/> as the body.
    /// </summary>
    [SKFunction("Send an email to one or more recipients.")]
    [SKFunctionInput(Description = "Email content/body")]
    [SKFunctionContextParameter(Name = Parameters.Recipients, D