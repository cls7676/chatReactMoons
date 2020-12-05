// Copyright (c) Microsoft. All rights reserved.

using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Skills.MsGraph.Diagnostics;

namespace Microsoft.SemanticKernel.Skills.MsGraph;

/// <summary>
/// Cloud drive skill (e.g. OneDrive).
/// </summary>
public class CloudDriveSkill
{
    /// <summary>
    /// <see cref="ContextVariables"/> parameter names.
    /// </summary>
    public static class Parameters
    {
        /// <summary>
        /// Document file path.
        /// </summary>
        public const string DestinationPath = "destinationPath";
    }

    private readonly ICloudDriveConnector _connector;
    private readonly ILogger<CloudDriveSkill> _logger;

    public CloudDriveSkill(ICloudDriveConnector connector, ILogger<CloudDriveSkill>? logger = null)
    {
        Ensure.NotNull(conne