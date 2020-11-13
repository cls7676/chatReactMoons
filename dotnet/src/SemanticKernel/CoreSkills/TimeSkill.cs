// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Globalization;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Microsoft.SemanticKernel.CoreSkills;

/// <summary>
/// TimeSkill provides a set of functions to get the current time and date.
/// </summary>
/// <example>
/// Usage: kernel.ImportSkill("time", new TimeSkill());
/// Examples:
/// {{time.date}}            => Sunday, 12 January, 2031
/// {{time.today}}           => Sunday, 12 January, 2031
/// {{time.now}}             => Sunday, January 12, 2031 9:15 PM
/// {{time.utcNow}}          => Sunday, January 13, 2031 5:15 AM
/// {{time.time}}            => 09:15:07 PM
/// {{time.year}}            => 2031
/// {{time.month}}           => January
/// {{time.monthNumber}}     => 01
/// {{time.day}}             => 12
/// {{time.dayOfMonth}}      => 12
/// {{time.dayOfWeek}}       => Sunday
/// {{time.hour}}            => 9 PM
/// {{time.hourNumber}}      => 21
/// {{ti