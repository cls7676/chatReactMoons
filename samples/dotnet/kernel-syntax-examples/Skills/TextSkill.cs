// Copyright (c) Microsoft. All rights reserved.

using System.Diagnostics.CodeAnalysis;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Skills;

public class TextSkill
{
    [SKFunction("Remove spaces to the left of a string")]
    [SKFunctionInput(Description = "Text to edit")]
    public string LStrip(string input)
    {
        return input.TrimStart();
    }

    [SKFunction("Remove spaces to the right of a string")]
    [SKFunctionInput(Description = "Text to edit")]
    public string RStrip(string input)
    {
        return input.TrimEnd();
    }

    [SKFunction("Remove spaces to the left and right of a string")]
    [SKFun