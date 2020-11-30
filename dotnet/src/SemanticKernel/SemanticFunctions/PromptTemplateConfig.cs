// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel.Diagnostics;
using Microsoft.SemanticKernel.Text;

namespace Microsoft.SemanticKernel.SemanticFunctions;

/// <summary>
/// Prompt template configuration.
/// </summary>
public class PromptTemplateConfig
{
    /// <summary>
    /// Completion configuration parameters.
    /// </summary>
    public class CompletionConfig
    {
        /// <summary>
        /// Sampling temperature to use, between 0 and 2. Higher values will make the output more random.
        /// Lower values will make it more focused and deterministic.
        /// </summary>
        [JsonPropertyName("temperature")]
        [JsonPropertyOrder(1)]
        public double Temperature { get; set; } = 0.0f;

        /// <summary>
        /// Cut-off of top_p probability mass of tokens to consider.
        /// For example, 0.1 means only the tokens comprising the top 10% probability mass are considered.
        /// </summary>
        [JsonPropertyName("top_p")]
        [JsonPropertyOrder(2)]
        public double TopP { get; set; } = 0.0f;

        /// <summary>
        /// Lowers the probability of a word appearing if it already appeared in the predicted text.
        /// Unlike the frequency penalty, the presence penalty does not depend on the frequency at which words
        /// appear in past predictions.
        /// </summary>
        [JsonPropertyName("presence_penalty")]
        [JsonPropertyOrder(3)]
        public double PresencePenalty { get; set; } = 0.0f;

        /// <summary>
        /// Controls the model’s tendency to repeat predictions. The frequency penalty reduces the probability
        /// of words that have already been generated. The penalty depends on how many times a word has already
        /// occurred in the prediction.
  