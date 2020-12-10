// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Skills.MsGraph;
using Microsoft.SemanticKernel.Skills.MsGraph.Models;
using Moq;
using Xunit;
using Xunit.Abstractions;
using static Microsoft.SemanticKernel.Skills.MsGraph.CalendarSkill;

namespace SemanticKernel.Skills.UnitTests.MsGraph;

public class CalendarSkillTests : IDisposable
{
    private readonly XunitLogger<SKContext> _logger;
    private readonly SKContext _context;

    public CalendarSkillTests(ITestOutputHelper output)
    {
        this._logger = new XunitLogger<SKContext>(output);
        this._context = new SKContext(new ContextVariables(), NullMemory.Instance, null, this._logger);
    }

    [Fact]
    public async Task AddEventAsyncSucceedsAsync()
    {
        // Arrange
        string anyContent = Guid.NewGuid().ToString();
        string anySubject = Guid.NewGuid().ToString();
        string anyLocation = Guid.NewGuid().ToString();
        DateTimeOffset anyStartTime = DateTimeOffset.Now + TimeSpan.FromDays(1);
        DateTimeOffset anyEndTime = DateTimeOffset.Now + TimeSpan.FromDays(1.1);
        string[] anyAttendees = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

        CalendarEvent expected = new(anySubject, anyStartTime, anyEndTime)
        {
            Content = anyContent,
            Location = anyLocation,
            Attendees = anyAttendees
        };

        Mock<ICalendarConnector> connectorMock = new();
        connectorMock.Setup(c => c.AddEventAsync(It.IsAny<CalendarEvent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        CalendarSkill target = new(connectorMock.Object);

        this._context.Variables.Set(Parameters.Start, anyStartTime.ToString(CultureInfo.InvariantCulture.DateTimeFormat));
        this._context.Variables.Set(Parameters.End, anyEndTime.ToString(CultureInfo.InvariantCulture.DateTimeFormat));
        this._context.Variables.Set(Parameters.Location, anyLocation);
        this._context.Variables.Set(Parameters.Content, anyCon