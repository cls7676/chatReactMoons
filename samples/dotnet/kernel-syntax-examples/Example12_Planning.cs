// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.KernelExtensions;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Orchestration.Extensions;
using Reliability;
using RepoUtils;
using Skills;

// ReSharper disable once InconsistentNaming

internal static class Example12_Planning
{
    public static async Task RunAsync()
    {
        await PoetrySamplesAsync();
        await EmailSamplesAsync();
        await BookSamplesAsync();
    }

    private static async Task PoetrySamplesAsync()
    {
        Console.WriteLine("======== Planning - Create and Execute Poetry Plan ========");
        var kernel = InitializeKernelAndPlanner(out var planner);

        // Load additional skills to enable planner to do non-trivial asks.
        string folder = RepoFiles.SampleSkillsPath();
        kernel.ImportSemanticSkillFromDirectory(folder, "SummarizeSkill");
        kernel.ImportSemanticSkillFromDirectory(folder, "WriterSkill");

        var originalPlan = await kernel.RunAsync("Write a poem about John Doe, then translate it into Italian.", planner["CreatePlan"]);
        // <goal>
        // Write a poem about John Doe, then translate it into Italian.
        // </goal>
        // <plan>
        //   <function.WriterSkill.ShortPoem input="John Doe is a kind and generous man who loves to help others and make them smile."/>
        //   <function.WriterSkill.Translate language="Italian"/>
        // </plan>

        Console.WriteLine("Original plan:");
        Console.WriteLine(originalPlan.Variables.ToPlan().PlanString);

        _ = await ExecutePlanAsync(kernel, planner, originalPlan, 5);
    }

    private static async Task EmailSamplesAsync()
    {
        Console.WriteLine("======== Planning - Create and Execute Email Plan ========");
        var kernel = InitializeKernelAndPlanner(out var planner);
        kernel.ImportSkill(new EmailSkill(), "email");

        // Load additional skills to enable planner to do non-trivial asks.
        string folder = RepoFiles.SampleSkillsPath();
        kernel.ImportSemanticSkillFromDirectory(folder, "SummarizeSkill");
        kernel.ImportSemanticSkillFromDirectory(folder, "WriterSkill");

        var originalPlan = await kernel.RunAsync("Summarize an input, translate to french, and e-mail to John Doe", planner["CreatePlan"]);
        // <goal>
        // Summarize an input, translate to french, and e-mail to John Doe
        // </goal>
        // <plan>
        //   <function.SummarizeSkill.Summarize/>
        //   <function.WriterSkill.Translate language="French" setContextVariable="TRANSLATED_SUMMARY"/>
        //   <function.email.GetEmailAddress input="John Doe" setContextVariable="EMAIL_ADDRESS"/>
        //   <function.email.SendEmail input="$TRANSLATED_SUMMARY" email_address="$EMAIL_ADDRESS"/>
        // </plan>

        Console.WriteLine("Original plan:");
        Console.WriteLine(originalPlan.Variables.ToPlan().PlanString);

        var executionResults = originalPlan;
        executionResults.Variables.Update(
            "Once upon a time, in a faraway kingdom, there lived a kind and just king named Arjun. " +
            "He ruled over his kingdom with fairness and compassion, earning him the love and admiration of his people. " +
            "However, the kingdom was plagued by a terrible dragon that lived in the nearby mountains and terrorized the nearby villag