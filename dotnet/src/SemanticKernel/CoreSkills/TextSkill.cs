// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel.SkillDefinition;

namespace Microsoft.SemanticKernel.CoreSkills;

/// <summary>
/// TextSkill provides a set of functions to manipulate strings.
/// </summary>
/// <example>
/// Usage: kernel.ImportSkill("text", new TextSkill());
///
/// Examples:
/// SKContext["input"] = "  hello world  "
/// {{text.trim $input}} => "hello world"
/// {{text.trimStart $input} => "hello world  "
/// {{text.trimEnd $input} => "  hello world"
/// SKContext["input"] = "hello world"
/// {{text.uppercase $input}} => "HELLO WORLD"
/// SKContext["input"] = "HELLO WORLD"
/// {{text.lowercase $input}} => "hello world"
/// </example>
public class TextSkill
{
    /// <summary>
    /// Trim whitespace from the start and end of a string.
    /// </summary>
    /// <example>
    /// SKContext["input"] = "  hello world  "
    /// {{text.trim $input}} => "hello world"
    /// </example>
    /// <param name="text"> The string to trim. </param>
    /// <returns> The trimmed string. </returns>
    [SKFunction("Trim whitespace from the start and end of a string.")]
    public string Trim(string text)
    {
        return text.Trim();
    }

    /// <summary>
    /// Trim whitespace from the start of a string.
    /// </summary>
    /// <example>
    /// SKContext["input"] = "  hello world  "
    /// {{text.trimStart $input} => "hello world  "
    /// </example>
    /// <param name="text"> The string to trim. </param>
    /// <returns> The trimmed string. </returns>
    [SKFunction("Trim whitespace from the start of a string.")]
    public string TrimStart(string text)
    {
        return text.TrimStart();
    }

    /// <summary>
    /// Trim whitespace from the end of a string.
    /// </summary>
    /// <example>
    /// SKContext["input"] = "  hello world  "
    /// {{te