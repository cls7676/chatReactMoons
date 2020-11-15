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
/// {{time.minute}}          => 15
/// {{time.minutes}}         => 15
/// {{time.second}}          => 7
/// {{time.seconds}}         => 7
/// {{time.timeZoneOffset}}  => -08:00
/// {{time.timeZoneName}}    => PST
/// </example>
/// <remark>
/// Note: the time represents the time on the hw/vm/machine where the kernel is running.
/// TODO: import and use user's timezone
/// </remark>
public class TimeSkill
{
    /// <summary>
    /// Get the current date
    /// </summary>
    /// <example>
    /// {{time.date}} => Sunday, 12 January, 2031
    /// </example>
    /// <returns> The current date </returns>
    [SKFunction("Get the current date")]
    public string Date()
    {
        // Example: Sunday, 12 January, 2025
        return DateTimeOffset.Now.ToString("D", CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Get the current date and time in the local time zone"
    /// </summary>
    /// <example>
    /// {{time.now}} => Sunday, January 12, 2025 9:15 PM
    /// </example>
    /// <returns> The current date and time in the local time zone </returns>
    [SKFunction("Get the current date and time in the local time zone")]
    public string Now()
    {
        // Sunday, January 12, 2025 9:15 PM
        return DateTimeOffset.Now.ToString("f", CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Get the current UTC date and time
    /// </summary>
    /// <example>
    /// {{time.utcNow}} => Sunday, January 13, 2025 5:15 AM
    /// </example>
    /// <returns> The current UTC date and time </returns>
    [SKFunction("Get the current UTC date and time")]
    public string UtcNow()
    {
        // Sunday, January 13, 2025 5:15 AM
        return DateTimeOffset.UtcNow.ToString("f", CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Get the current time
    /// </summary>
    /// <example>
    /// {{time.time}} => 09:15:07 PM
    /// </example>
    /// <returns> The current time </returns>
    [SKFunction("Get the current time")]
    public string Time()
    {
        // Example: 09:15:07 PM
        return DateTimeOffset.Now.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);
    }

    /// <summary