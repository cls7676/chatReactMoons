// Copyright (c) Microsoft. All rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Microsoft.SemanticKernel.Skills.Web;

/// <summary>
/// Get search URLs for various websites
/// </summary>
[SuppressMessage("Design", "CA1055:URI return values should not be strings", Justification = "Semantic Kernel operates on strings")]
public class SearchUrlSkill
{
    /**
     * Amazon Search URLs
     */
    /// <summary>
    /// Get search URL for Amazon
    /// </summary>
    [SKFunction("Return URL for Amazon search query")]
    public string AmazonSearchUrl(string query)
    {
        string encoded = UrlEncoder.Default.Encode(query);
        return $"https://www.amazon.com/s?k={encoded}";
    }

    /**
     * Bing Search URLs
     */
    /// <summary>
    /// Get search URL for Bing
    /// </summary>
    [SKFunction("Return URL for Bing search query.")]
    [SKFunctionInput(Description