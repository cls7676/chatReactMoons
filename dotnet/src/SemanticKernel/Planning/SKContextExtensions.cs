// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Orchestration.Extensions;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Microsoft.SemanticKernel.Planning;

internal static class SKContextExtensions
{
    internal static string GetFunctionsManual(
        this SKContext context,
        List<string>? excludedSkills = null,
        List<string>? excludedFunctions = null)
    {
        var functions = context.GetAvailableFunctions(excludedSkills, excludedFunctions);

        return string.Join("\n\n",
            functions.Select(
                x =>
                {
                    var inputs = string.Join("\n", x.Parameters.Select(p => 