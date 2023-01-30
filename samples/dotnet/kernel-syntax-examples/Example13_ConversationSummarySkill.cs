// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.Orchestration;
using Reliability;
using RepoUtils;

// ReSharper disable once InconsistentNaming

internal static class Example13_ConversationSummarySkill
{
    private const string ChatTranscript =
        @"
John: Hello, how are you?
Jane: I'm fine, thanks. How are you?
John: I'm doing well, writing some example code.
Jane: That's great! I'm writing some example code too.
John: What are you writing?
Jane: I'm writing a chatbot.
John: That's cool. I'm writing a chatbot too.
Jane: What language are you writing it in?
John: I'm writing it in C#.
Jane: I'm writing it in Python.
John: That's cool. I need to learn Python.
Jane: I need to learn C#.
John: Can I try out your chatbot?
Jane: Sure, here's the link.
John: Thanks!
Jane: You're welcome.
Jane: Look at this poem my chatbot wrote:
Jane: Roses are red
Jane: Violets are blue
Jane: I'm writing a chatbot
Jane: What about you?
John: That's cool. Let me see if mine will write a poem, too.
John: Here's a poem my chatbot wrote:
John: The signularity of the universe is a mystery.
John: The universe is a mystery.
John: The universe is a mystery.
John: The universe is a mystery.
John: Looks like I need to improve mine, oh well.
Jane: You might want to try using a different model.
Jane: I'm using the GPT-3 model.
John: I'm using the GPT-2 model. That makes sense.
John: Here is a new poem after updating the model.
John: The universe is a mystery.
John: The universe is a mystery.
John: The universe is a mystery.
John: Yikes, it's really stuck isn't it. Would you help me debug my code?
Jane: Sure, what's the problem?
John: I'm not sure. I think it's a bug in the code.
Jane: I'll take a look.
Jane: I think I found the problem.
Jane: It looks like you're not passing the right parameters to the model.
John: Thanks for the help!
Jane: I'm now writing a bot to summarize conversations. I want to make sure it works when the conversation is long.
John: So you need to keep talking with me to generate a long conversation?
Jane: Yes, that's right.
John: Ok, I'll keep talking. What should we talk about?
Jane: I don't know, what do you want to talk about?
John: I don't know, it's nice how CoPilot is doing most of the talking for us. But it definitely gets stuc