
﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.KernelExtensions;
using Microsoft.SemanticKernel.SkillDefinition;
using RepoUtils;
using Skills;

// ReSharper disable once InconsistentNaming
public static class Example10_DescribeAllSkillsAndFunctions
{
    /// <summary>
    /// Print a list of all the functions imported into the kernel, including function descriptions,
    /// list of parameters, parameters descriptions, etc.
    /// See the end of the file for a sample of what the output looks like.
    /// </summary>
    public static void Run()
    {
        Console.WriteLine("======== Describe all skills and functions ========");

        var kernel = KernelBuilder.Create();
        kernel.Config.AddOpenAICompletionBackend("davinci", "text-davinci-003", "none");

        // Import a native skill
        var skill1 = new StaticTextSkill();
        kernel.ImportSkill(skill1, "StaticTextskill");

        // Import another native skill
        var skill2 = new TextSkill();
        kernel.ImportSkill(skill2, "AnotherTextskill");

        // Import a semantic skill
        string folder = RepoFiles.SampleSkillsPath();
        kernel.ImportSemanticSkillFromDirectory(folder, "SummarizeSkill");

        // Define a semantic function inline, without naming
        var sFun1 = kernel.CreateSemanticFunction("tell a joke about {{$input}}", maxTokens: 150);

        // Define a semantic function inline, with skill name
        var sFun2 = kernel.CreateSemanticFunction(
            "write a novel about {{$input}} in {{$language}} language",
            skillName: "Writing",
            functionName: "Novel",
            description: "Write a bedtime story",
            maxTokens: 150);

        FunctionsView functions = kernel.Skills.GetFunctionsView();
        ConcurrentDictionary<string, List<FunctionView>> nativeFunctions = functions.NativeFunctions;
        ConcurrentDictionary<string, List<FunctionView>> semanticFunctions = functions.SemanticFunctions;

        Console.WriteLine("*****************************************");
        Console.WriteLine("****** Native skills and functions ******");
        Console.WriteLine("*****************************************");
        Console.WriteLine();

        foreach (KeyValuePair<string, List<FunctionView>> skill in nativeFunctions)
        {
            Console.WriteLine("Skill: " + skill.Key);
            foreach (FunctionView func in skill.Value) { PrintFunction(func); }
        }

        Console.WriteLine("*****************************************");
        Console.WriteLine("***** Semantic skills and functions *****");
        Console.WriteLine("*****************************************");
        Console.WriteLine();

        foreach (KeyValuePair<string, List<FunctionView>> skill in semanticFunctions)
        {
            Console.WriteLine("Skill: " + skill.Key);
            foreach (FunctionView func in skill.Value) { PrintFunction(func); }
        }
    }

    private static void PrintFunction(FunctionView func)
    {
        Console.WriteLine($"   {func.Name}: {func.Description}");

        if (func.Parameters.Count > 0)
        {
            Console.WriteLine("      Params:");
            foreach (var p in func.Parameters)
            {
                Console.WriteLine($"      - {p.Name}: {p.Description}");
                Console.WriteLine($"        default: '{p.DefaultValue}'");
            }
        }

        Console.WriteLine();
    }
}

#pragma warning disable CS1587 // XML comment is not placed on a valid language element
/** Sample output:

*****************************************
****** Native skills and functions ******
*****************************************

Skill: StaticTextskill
   Uppercase: Change all string chars to uppercase
      Params:
      - input: Text to uppercase
        default: ''

   AppendDay: Append the day variable
      Params:
      - input: Text to append to
        default: ''
      - day: Value of the day to append
        default: ''

Skill: AnotherTextskill
   Uppercase: Change all string chars to uppercase
      Params:
      - input: Text to uppercase
        default: ''

   Strip: Remove spaces to the left and right of a string
      Params:
      - input: Text to edit
        default: ''

   LStrip: Remove spaces to the left of a string
      Params:
      - input: Text to edit
        default: ''

   RStrip: Remove spaces to the right of a string
      Params:
      - input: Text to edit
        default: ''

   Lowercase: Change all string chars to lowercase