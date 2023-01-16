﻿// Copyright (c) Microsoft. All rights reserved.

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
  <function.WriterSkill.NovelChapter input="$CHAPTER_3" theme="Mystery" previousChapter="$CHAPTER_2" chapterIndex="3" appendToResult="RESULT__CHAPTER_3" />
</plan>
warn: object[0]
      Variable `$previousChapter` not found
Step 3 - Execution results:
<goal>
Create a book with 3 chapters about a group of kids in a club called 'The Thinking Caps.'
</goal><plan>
  <function.WriterSkill.NovelChapter input="$CHAPTER_2" theme="Mystery" previousChapter="$CHAPTER_1" chapterIndex="2" appendToResult="RESULT__CHAPTER_2" />
  <function.WriterSkill.NovelChapter input="$CHAPTER_3" theme="Mystery" previousChapter="$CHAPTER_2" chapterIndex="3" appendToResult="RESULT__CHAPTER_3" />
</plan>
Step 4 - Execution results:
<goal>
Create a book with 3 chapters about a group of kids in a club called 'The Thinking Caps.'
</goal><plan>
  <function.WriterSkill.NovelChapter input="$CHAPTER_3" theme="Mystery" previousChapter="$CHAPTER_2" chapterIndex="3" appendToResult="RESULT__CHAPTER_3" />
</plan>
Step 5 - Execution results:
<goal>
Create a book with 3 chapters about a group of kids in a club called 'The Thinking Caps.'
</goal><plan>
</plan>
Step 5 - COMPLETE!


RESULT__CHAPTER_1

The Thinking Caps were sitting in their usual spot in the library, surrounded by books, papers, and gadgets. They were busy studying and preparing for the upcoming trivia contest, which was only a few days away.

"Okay, team, let's review what we've learned so far," Max said, holding a clipboard and a pen. He was wearing a blue cap with a light bulb logo, which matched his bright eyes and blond hair. "Lily, what is the capital of Peru?"

"Lima," Lily answered without hesitation. She was wearing a purple cap with a paintbrush logo, which complemented her long brown hair and artistic flair. She was also holding a sketchbook and a pencil, where she had drawn a map of South America and some of its landmarks.

"Correct. Sam, what is the name of the device that measures atmospheric pressure?"

"A barometer," Sam replied confidently. He was wearing a green cap with a wrench logo, which suited his short black hair and inventive mind. He was also holding a small gadget that he had made out of spare parts, which he claimed could measure the temperature, humidity, and wind speed.

"Correct. Mia, what is the term for a word that sounds the same as another word but has a different meaning and spelling?"

"A homophone," Mia said with a smile. She was wearing a red cap with a pen logo, which matched her curly red hair and journalistic spirit. She was also holding a notebook and a recorder, where she had written down some trivia questions and recorded some interviews with the school staff and students.

"Correct. You guys are amazing. We're going to ace this contest for sure," Max said, giving them a thumbs up. "We've covered geography, science, and language. What's next?"

"How about history?" Lily suggested. "That's always fun."

"Good idea. Let's see, who can tell me who was the first president of the United States?" Max asked, looking at his clipboard.

"George Washington," the four friends said in unison.

"Too easy. How about who was the first president of France?" Max asked, raising the difficulty level.

"Napoleon Bonaparte," Sam said, raising his hand.

"Wrong. He was the first emperor of France, not the president. The first president was Louis-Napoleon Bonaparte, his nephew," Mia corrected him, showing off her knowledge.

"Wow, Mia, you're a history buff. How do you know all this stuff?" Sam asked, impressed.

"I read a lot of books and magazines. And I watch a lot of documentaries and podcasts. History is fascinating. It's like a giant mystery waiting to be solved," Mia explained, her eyes sparkling.

"I agree. History is full of clues and secrets. And speaking of mysteries, do you guys know what's the biggest mystery in our school right now?" Max asked, changing the topic.

"What?" the others asked, curious.

"The case of the missing mascot," Max said, lowering his voice and looking around. "Have you heard about it?"

"Of course we have. Everyone has. It's been the talk of the school for the past week," Lily said, nodding.

"For those who don't know, our school mascot, Hootie the owl, has been stolen from the principal's office. Hootie is a stuffed animal that has been with the school for over 50 years. He's not only a symbol of our school spirit, but also a lucky charm for us, the Thinking Caps. We always bring him along to our competitions, and we always win," Max explained, sounding proud and sad at the same time.

"Who would do such a thing? Who would steal Hootie?" Sam asked, shaking his head.

"That's the mystery. No one knows who did it, or why, or how. The principal's office was locked and alarmed, and there was no sign of forced entry or exit. The only clue was a note that was left on the principal's desk. It said: 'Hootie is gone. You'll never see him again. Good luck with the trivia contest. You'll need it.' And it was signed with a question mark," Max said, showing them a copy of the note that he had obtained from Mia.

"That's creepy. And mean. And rude. And unfair. And..." Lily said, running out of adjectives.

"And challenging," Max said, interrupting her. "Don't you see? This is a challenge for us, the Thinking Caps. Someone is trying to mess with us, to sabotage our chances of winning the contest. Someone who thinks they can outsmart us, outwit us, outplay us. Someone who wants to see us fail."

"Who could it be? Who hates us that much?" Sam asked, looking around suspiciously.

"It could be anyone. It could be the chess club, our main rivals in the contest. They're always jealous of our skills and achievements. They're always trying to beat us and prove that they're smarter than us," Max said, narrowing his eyes.

"Or it could be the class clown, the one who's always pulling pranks and jokes on everyone. He's always looking for a laugh and a thrill. He's always trying to get attention and cause trouble," Lily said, frowning.

"Or it could be the new kid, the one who's always quiet and mysterious. He's always alone and secretive. He's always hiding something and watching everything," Mia said, pointing at a corner where a boy with dark hair and glasses was sitting by himself, reading a book.

"Or it could be someone else, someone we don't know, someone we don't expect, someone who's hiding in plain sight," Sam said, widening his eyes.

"Whoever it is, we're not going to let them get away with it. We're not going to let them steal our mascot and our glory. We're not going to let them win. We're going to find Hootie and bring him back. We're going to solve this mystery and win this contest. We're going to show them who the real Thinking Caps are," Max said, raising his voice and his fist.

"Yeah!" the others said, joining him.

"Are you with me, team?" Max asked, looking at them.

"Always," they said, nodding.

"Then let's do this. Let's crack this case. Let's find the missing mascot. Let's go, Thinking Caps!" Max said, leading them out of the library.

"Let's go, Thinking Caps!" they repeated, following him.

And so, the adventure began. The Case of the Missing Mascot. The first mystery of the Thinking Caps. The first challenge of their lives. The first chapter of their story.

RESULT__CHAPTER_2

The Thinking Caps were sitting at their usual table in the library, surrounded by books, papers, and laptops. They were preparing for their next trivia contest, which was only a week away. They had already won the first round, thanks to their knowledge and teamwork. They were determined to win the second round, too, and make it to the finals.

"Okay, team, let's review what we've learned so far," Max said, holding a clipboard and a pen. He was the leader and the brains of the group, and he always had a plan. "The topic for the second round is history. We've covered ancient civilizations, medieval times, and the American Revolution. What's next?"

"How about the French Revolution?" Lily suggested, raising her hand. She was the artist and the heart of the group, and she loved to draw and paint. She had a sketchbook and a pencil in front of her, where she had doodled some scenes from the history books. "It's one of my favorite periods. There was so much drama, romance, and art."

"Good idea, Lily," Max said, nodding. "The French Revolution was a pivotal moment in world history. It changed the course of politics, culture, and society. There are a lot of facts and dates to remember, though. We'll need to study hard."

"I've got that covered," Sam said, smiling. He was the inventor and the hands of the group, and he liked to build and tinker with gadgets and machines. He had a laptop and a pair of headphones on the table, where he had created a quiz app to help them memorize the information. "I've programmed a fun and interactive way to learn about the French Revolution. It's called 'Guillotine or Not'. You have to answer questions correctly, or else you lose your head."

"Sounds gruesome, but effective," Mia said, chuckling. She was the journalist and the voice of the group, and she enjoyed writing and reporting. She had a notebo