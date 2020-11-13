
﻿// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.SemanticKernel.CoreSkills;

internal static class SemanticFunctionConstants
{
    internal const string FunctionFlowFunctionDefinition =
        @"Create an XML plan step by step, to satisfy the goal given.
To create a plan, follow these steps:
1. From a <goal> create a <plan> as a series of <functions>.
2. Use only the [AVAILABLE FUNCTIONS] - do not create new functions, inputs or attribute values.
3. Only use functions that are required for the given goal.
4. A function has an $input and an $output.
5. The $output from each function is automatically passed as $input to the subsequent <function>.
6. $input does not need to be specified if it consumes the $output of the previous function.
7. To save an $output from a <function>, to pass into a future <function>, use <function.{FunctionName} ... setContextVariable: ""$<UNIQUE_VARIABLE_KEY>""/>
8. To save an $output from a <function>, to return as part of a plan result, use <function.{FunctionName} ... appendToResult: ""RESULT__$<UNIQUE_RESULT_KEY>""/>
9. Append an ""END"" XML comment at the end of the plan.

Here are some good examples:

[AVAILABLE FUNCTIONS]
  WriterSkill.Summarize:
    description: summarize input text
    inputs:
    - $input: the text to summarize
  LanguageHelpers.TranslateTo:
    description: translate the input to another language
    inputs:
    - $input: the text to translate
    - $translate_to_language: the language to translate to
  EmailConnector.LookupContactEmail:
    description: looks up the a contact and retrieves their email address
    inputs:
    - $input: the name to look up
  EmailConnector.EmailTo:
    description: email the input text to a recipient
    inputs:
    - $input: the text to email
    - $recipient: the recipient's email address. Multiple addresses may be included if separated by ';'.
[END AVAILABLE FUNCTIONS]

<goal>Summarize the input, then translate to japanese and email it to Martin</goal>
<plan>
  <function.WriterSkill.Summarize/>
  <function.LanguageHelpers.TranslateTo translate_to_language=""Japanese"" setContextVariable=""TRANSLATED_TEXT"" />
  <function.EmailConnector.LookupContactEmail input=""Martin"" setContextVariable=""CONTACT_RESULT"" />
  <function.EmailConnector.EmailTo input=""$TRANSLATED_TEXT"" recipient=""$CONTACT_RESULT""/>
</plan><!-- END -->

[AVAILABLE FUNCTIONS]
  AuthorAbility.Summarize:
    description: summarizes the input text
    inputs:
    - $input: the text to summarize
  Magician.TranslateTo:
    description: translate the input to another language
    inputs:
    - $input: the text to translate
    - $translate_to_language: the language to translate to
  _GLOBAL_FUNCTIONS_.GetEmailAddress:
    description: Gets email address for given contact
    inputs:
    - $input: the name to look up
  _GLOBAL_FUNCTIONS_.SendEmail:
    description: email the input text to a recipient
    inputs:
    - $input: the text to email
    - $recipient: the recipient's email address. Multiple addresses may be included if separated by ';'.
[END AVAILABLE FUNCTIONS]

<goal>Summarize an input, translate to french, and e-mail to John Doe</goal>
<plan>
    <function.AuthorAbility.Summarize/>
    <function.Magician.TranslateTo translate_to_language=""French"" setContextVariable=""TRANSLATED_SUMMARY""/>
    <function._GLOBAL_FUNCTIONS_.GetEmailAddress input=""John Doe"" setContextVariable=""EMAIL_ADDRESS""/>
    <function._GLOBAL_FUNCTIONS_.SendEmail input=""$TRANSLATED_SUMMARY"" email_address=""$EMAIL_ADDRESS""/>
</plan><!-- END -->

[AVAILABLE FUNCTIONS]
  Everything.Summarize:
    description: summarize input text
    inputs:
    - $input: the text to summarize
  _GLOBAL_FUNCTIONS_.NovelOutline :
    description: Outlines the input text as if it were a novel
    inputs:
    - $input: the title of the novel to outline
    - $chapterCount: the number of chapters to outline
  Emailer.EmailTo:
    description: email the input text to a recipient
    inputs:
    - $input: the text to email
    - $recipient: the recipient's email address. Multiple addresses may be included if separated by ';'.
[END AVAILABLE FUNCTIONS]

<goal>Create an outline for a children's book with 3 chapters about a group of kids in a club and then summarize it.</goal>
<plan>
  <function._GLOBAL_FUNCTIONS_.NovelOutline input=""A group of kids in a club called 'The Thinking Caps' that solve mysteries and puzzles using their creativity and logic."" chapterCount=""3"" />
  <function.Everything.Summarize/>
</plan><!-- END -->

End of examples.

[AVAILABLE FUNCTIONS]
{{$available_functions}}
[END AVAILABLE FUNCTIONS]

<goal>{{$input}}</goal>
";

    internal const string BucketFunctionDefinition =
        @"1. Given an output of a function, bucket the output into a list of results.

Examples:
[CONTENT]
Result 1
This is the first result.
Result 2
This is the second result. It's doubled!
Result 3
This is the third and final result. Truly astonishing.
[END CONTENT]

EXPECTED BUCKETS:

Result:
{""buckets"": [""Result 1
This is the first result."", ""Result 2
This is the second result. It's doubled!"", ""Result 3
This is the third and final result. Truly astonishing.""]}

End examples.

[CONTENT]
{{$input}}
[END CONTENT]

EXPECTED BUCKETS: {{$bucketCount}}

Result:
";

    internal const string ProblemSolverFunctionDefinition =
        @"Create a plan as a list, step by step, to answer the request or goal given.
To create a plan, follow these steps:
1. Identify the request or goal to be achieved.
2. Break down the request into smaller tasks and steps.
3. If the goal has a ""use"" parameter, use those functions with the exact name given.
4. Use experience and logic to determine the steps and tasks needed.
5. Provide a detailed decision-making process for each step.
6. Avoid generic instructions and add as many details as possible.
7. Present the solution using precise XML syntax with ""solution"" and ""step"" tags.
8. Ensure that all tags are closed.
9. Append an ""END"" comment at the end of the plan.
10. If you don't know how to fulfill a request, use the ""noSolution"" tag.
11. Use a computer, browser, apps, and external services to fulfill the goal.
12. Ensure that all goals are fulfilled from a computer.

Here some good examples:

<goal>
what time is it?
</goal>
<variables />
<solution>
  <step>Get current location</step>
  <step>Find the time zone for the location in the variables</step>
  <step>Get the current time for the time zone in the variables</step>
</plan><!-- END -->

<goal use=""time.timezone"">
what time is it?
</goal>
<variables />
<solution>
  <callFunction name=""time.timezone"" />
  <step>Get the current time for time zone in the variables</step>
</solution><!-- END -->

<goal use=""time.timezone,time.currentTime"">
what time is it?
</goal>
<variables />
<solution>
  <callFunction name=""time.timezone"" />
  <callFunction name=""time.currentTime"" />
  <step>Get the current time from the variables</step>
</solution><!-- END -->

<goal use=""timeSkill.GetTimezone,timeSkill.currentTime,timeSkill.currentDate"">
how long till Christmas?
</goal>
<variables />
<solution>
  <callFunction name=""timeSKill.GetTimezone"" />
  <callFunction name=""timeSKill.currentTime"" />
  <callFunction name=""timeSKill.currentDate"" />
  <step>Get the current date from the variables</step>
  <step>Calculate days from ""current date"" to ""December 25""</step>
</solution><!-- END -->

<goal>
Get user's location
</goal>
<variables />
<solution>
  <step>Search for the user location in variables</step>
  <step>If the user location is unknown ask the user: What is your location?</step>
</solution><!-- END -->