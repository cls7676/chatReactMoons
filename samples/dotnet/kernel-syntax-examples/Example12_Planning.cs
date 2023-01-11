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
            "However, the kingdom was plagued by a terrible dragon that lived in the nearby mountains and terrorized the nearby villages, " +
            "burning their crops and homes. The king had tried everything to defeat the dragon, but to no avail. " +
            "One day, a young woman named Mira approached the king and offered to help defeat the dragon. She was a skilled archer " +
            "and claimed that she had a plan to defeat the dragon once and for all. The king was skeptical, but desperate for a solution, " +
            "so he agreed to let her try. Mira set out for the dragon's lair and, with the help of her trusty bow and arrow, " +
            "she was able to strike the dragon with a single shot through its heart, killing it instantly. The people rejoiced " +
            "and the kingdom was at peace once again. The king was so grateful to Mira that he asked her to marry him and she agreed. " +
            "They ruled the kingdom together, ruling with fairness and compassion, just as Arjun had done before. They lived " +
            "happily ever after, with the people of the kingdom remembering Mira as the brave young woman who saved them from the dragon.");
        _ = await ExecutePlanAsync(kernel, planner, executionResults, 5);
    }

    private static async Task BookSamplesAsync()
    {
        Console.WriteLine("======== Planning - Create and Execute Book Creation Plan  ========");
        var kernel = InitializeKernelAndPlanner(out var planner);

        // Load additional skills to enable planner to do non-trivial asks.
        string folder = RepoFiles.SampleSkillsPath();
        kernel.ImportSemanticSkillFromDirectory(folder, "WriterSkill");

        var originalPlan = await kernel.RunAsync(
            "Create a book with 3 chapters about a group of kids in a club called 'The Thinking Caps.'",
            planner["CreatePlan"]);
        // <goal>
        // Create a book with 3 chapters about a group of kids in a club called 'The Thinking Caps.'
        // </goal>
        // <plan>
        //   <function.WriterSkill.NovelOutline input="A group of kids in a club called 'The Thinking Caps' solve mysteries and puzzles using their creativity and logic." chapterCount="3" endMarker="***" setContextVariable="OUTLINE" />
        //   <function.planning.BucketOutputs input="$OUTLINE" bucketCount="3" bucketLabelPrefix="CHAPTER" />
        //   <function.WriterSkill.NovelChapter input="$CHAPTER_1" theme="Mystery" chapterIndex="1" appendToResult="CHAPTER_1_TEXT" />
        //   <function.WriterSkill.NovelChapter input="$CHAPTER_2" theme="Mystery" previousChapter="$CHAPTER_1" chapterIndex="2" appendToResult="CHAPTER_2_TEXT" />
        //   <function.WriterSkill.NovelChapter input="$CHAPTER_3" theme="Mystery" previousChapter="$CHAPTER_2" chapterIndex="3" appendToResult="CHAPTER_3_TEXT" />
        // </plan>

        Console.WriteLine("Original plan:");
        Console.WriteLine(originalPlan.Variables.ToPlan().PlanString);

        Stopwatch sw = new();
        sw.Start();
        _ = await ExecutePlanAsync(kernel, planner, originalPlan);
    }

    private static IKernel InitializeKernelAndPlanner(out IDictionary<string, ISKFunction> planner)
    {
        var kernel = new KernelBuilder().WithLogger(ConsoleLogger.Log).Build();
        kernel.Config.AddAzureOpenAICompletionBackend(
            Env.Var("AZURE_OPENAI_DEPLOYMENT_LABEL"),
            Env.Var("AZURE_OPENAI_DEPLOYMENT_NAME"),
            Env.Var("AZURE_OPENAI_ENDPOINT"),
            Env.Var("AZURE_OPENAI_KEY"));
        kernel.Config.SetRetryMechanism(new RetryThreeTimesWithBackoff());

        // Load native skill into the kernel skill collection, sharing its functions with prompt templates
        planner = kernel.ImportSkill(new PlannerSkill(kernel), "planning");

        return kernel;
    }

    private static async Task<SKContext> ExecutePlanAsync(
        IKernel kernel,
        IDictionary<string, ISKFunction> planner,
        SKContext executionResults,
        int maxSteps = 10)
    {
        Stopwatch sw = new();
        sw.Start();

        // loop until complete or at most N steps
        for (int step = 1; !executionResults.Variables.ToPlan().IsComplete && step < maxSteps; step++)
        {
            var results = await kernel.RunAsync(executionResults.Variables, planner["ExecutePlan"]);
            if (results.Variables.ToPlan().IsSuccessful)
            {
                Console.WriteLine($"Step {step} - Execution results:");
                Console.WriteLine(results.Variables.ToPlan().PlanString);

                if (results.Variables.ToPlan().IsComplete)
                {
                    Console.WriteLine($"Step {step} - COMPLETE!");
                    Console.WriteLine(results.Variables.ToPlan().Result);

                    // Console.WriteLine("VARIABLES: ");
                    // Console.WriteLine(string.Join("\n\n", results.Variables.Select(v => $"{v.Key} = {v.Value}")));
                    break;
                }

                // Console.WriteLine($"Step {step} - Results so far:");
                // Console.WriteLine(results.ToPlan().Result);
            }
            else
            {
                Console.WriteLine($"Step {step} - Execution failed:");
                Console.WriteLine(results.Variables.ToPlan().Result);
                break;
            }

            executionResults = results;
        }

        sw.Stop();
        Console.WriteLine($"Execution complete in {sw.ElapsedMilliseconds} ms!");
        return executionResults;
    }
}

// ReSharper disable CommentTypo
/* Example Output:

======== Planning - Create and Execute Poetry Plan ========
Original plan:
<goal>
Write a poem about John Doe, then translate it into Italian.
</goal>
<plan>
  <function.WriterSkill.ShortPoem input="John Doe is a kind and generous man who loves to help others and make them smile."/>
  <function.WriterSkill.Translate language="Italian"/>
</plan>
Step 1 - Execution results:
<goal>
Write a poem about John Doe, then translate it into Italian.
</goal><plan>
  <function.WriterSkill.Translate language="Italian" />
</plan>
Step 2 - Execution results:
<goal>
Write a poem about John Doe, then translate it into Italian.
</goal><plan>
</plan>
Step 2 - COMPLETE!
John Doe è un uomo di grande cuore e grazia
Ha sempre un sorriso sul volto
Aiuta i poveri, i malati e i vecchi
Ma a volte la sua bontà lo mette nei guai
Come quando ha dato il suo cappotto a un tremante

- There once was a girl named Alice
Who loved to explore and be curious
She followed a rabbit down a burrow
And found a world of wonder and magic
But also of danger and madness

C'era una volta una ragazza di nome Alice
Che amava esplorare e essere curiosa
Seguì un coniglio in una buca
E trovò un mondo di meraviglia e magia
Ma anche di pericolo e follia

- Roses are red, violets are blue
Sugar is sweet, and so are you
But roses have thorns, and violets can fade
Sugar can spoil, and you can betray
So don't trust the words, but the actions that prove

Le rose sono rosse, le viole sono blu
Lo zucchero è dolce, e così sei tu
Ma le rose hanno spine, e le
Execution complete in 15223 ms!
======== Planning - Create and Execute Email Plan ========
Original plan:
<goal>
Summarize an input, translate to french, and e-mail to John Doe
</goal>
<plan>
    <function.SummarizeSkill.Summarize/>
    <function.WriterSkill.Translate language="French" setContextVariable="TRANSLATED_SUMMARY"/>
    <function.email.GetEmailAddress input="John Doe" setContextVariable="EMAIL_ADDRESS"/>
    <function.email.SendEmail input="$TRANSLATED_SUMMARY" email_address="$EMAIL_ADDRESS"/>
</plan>
Step 1 - Execution results:
<goal>
Summarize an input, translate to french, and e-mail to John Doe
</goal><plan>
  <function.WriterSkill.Translate language="French" setContextVariable="TRANSLATED_SUMMARY" />
  <function.email.GetEmailAddress input="John Doe" setContextVariable="EMAIL_ADDRESS" />
  <function.email.SendEmail input="$TRANSLATED_SUMMARY" email_address="$EMAIL_ADDRESS" />
</plan>
Step 2 - Execution results:
<goal>
Summarize an input, translate to french, and e-mail to John Doe
</goal><plan>
  <function.email.GetEmailAddress input="John Doe" setContextVariable="EMAIL_ADDRESS" />
  <function.email.SendEmail input="$TRANSLATED_SUMMARY" email_address="$EMAIL_ADDRESS" />
</plan>
Step 3 - Execution results:
<goal>
Summarize an input, translate to french, and e-mail to John Doe
</goal><plan>
  <function.email.SendEmail input="$TRANSLATED_SUMMARY" email_address="$EMAIL_ADDRESS" />
</plan>
Step 4 - Execution results:
<goal>
Summarize an input, translate to french, and e-mail to John Doe
</goal><plan>
</plan>
Step 4 - COMPLETE!
Sent email to: johndoe1234@example.com. Body: Some possible translations are:

- Mira, une archère habile, tue un dragon et épouse le roi qui régnait avec bonté.
- Un roi bienveillant et une archère courageuse vainquent un dragon et gouvernent le royaume ensemble.
- Le dragon qui terrorisait un beau royaume est abattu par une archère qui devient la femme du roi.
Execution complete in 9635 ms!
======== Planning - Create and Execute Book Creation Plan  ========
Original plan:
<goal>
Create a book with 3 chapters about a group of kids in a club called 'The Thinking Caps.'
</goal>
<plan>
  <function.WriterSkill.NovelOutline input="A group of kids in a club called 'The Thinking Caps' solve mysteries and puzzles using their creativity and logic." chapterCount="3" endMarker="***" setContextVariable="OUTLINE" />
  <function.planning.BucketOutputs input="$OUTLINE" bucketCount="3" bucketLabelPrefix="CHAPTER" />
  <function.WriterSkill.NovelChapter input="$CHAPTER_1" theme="Mystery" chapterIndex="1" appendToResult="RESULT__CHAPTER_1" />
  <function.WriterSkill.NovelChapter input="$CHAPTER_2" theme="Mystery" previousChapter="$CHAPTER_1" chapterIndex="2" appendToResult="RESULT__CHAPTER_2" />
  <function.WriterSkill.NovelChapter input="$CHAPTER_3" theme="Mystery" previousChapter="$CHAPTER_2" chapterIndex="3" appendToResult="RESULT__CHAPTER_3" />
</plan>
Step 1 - Execution results:
<goal>
Create a book with 3 chapters about a group of kids in a club called 'The Thinking Caps.'
</goal><plan>
  <function.planning.BucketOutputs input="$OUTLINE" bucketCount="3" bucketLabelPrefix="CHAPTER" />
  <function.WriterSkill.NovelChapter input="$CHAPTER_1" theme="Mystery" chapterIndex="1" appendToResult="RESULT__CHAPTER_1" />
  <function.WriterSkill.NovelChapter input="$CHAPTER_2" theme="Mystery" previousChapter="$CHAPTER_1" chapterIndex="2" appendToResult="RESULT__CHAPTER_2" />
  <function.WriterSkill.NovelChapter input="$CHAPTER_3" theme="Mystery" previousChapter="$CHAPTER_2" chapterIndex="3" appendToResult="RESULT__CHAPTER_3" />
</plan>
Step 2 - Execution results:
<goal>
Create a book with 3 chapters about a group of kids in a club called 'The Thinking Caps.'
</goal><plan>
  <function.WriterSkill.NovelChapter input="$CHAPTER_1" theme="Mystery" chapterIndex="1" appendToResult="RESULT__CHAPTER_1" />
  <function.WriterSkill.NovelChapter input="$CHAPTER_2" theme="Mystery" previousChapter="$CHAPTER_1" chapterIndex="2" appendToResult="RESULT__CHAPTER_2" />
  <function.WriterSkill.NovelChapter input="$CHAPTER_3" theme="Mystery" previousChapt