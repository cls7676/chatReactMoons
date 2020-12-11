
﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Skills.MsGraph;
using Microsoft.SemanticKernel.Skills.MsGraph.Models;
using Moq;
using Xunit;
using static Microsoft.SemanticKernel.Skills.MsGraph.TaskListSkill;

namespace SemanticKernel.Skills.UnitTests.MsGraph;

public class TaskListSkillTests
{
    private readonly SKContext _context = new SKContext(new ContextVariables(), NullMemory.Instance, null, NullLogger.Instance);

    private readonly TaskManagementTaskList _anyTaskList = new TaskManagementTaskList(
        id: Guid.NewGuid().ToString(),
        name: Guid.NewGuid().ToString());

    private readonly TaskManagementTask _anyTask = new TaskManagementTask(
        id: Guid.NewGuid().ToString(),
        title: Guid.NewGuid().ToString(),
        reminder: (DateTimeOffset.Now + TimeSpan.FromDays(1)).ToString("o"),
        due: DateTimeOffset.Now.ToString("o"),
        isCompleted: false);

    [Fact]
    public async Task AddTaskAsyncNoReminderSucceedsAsync()
    {
        // Arrange
        string anyTitle = Guid.NewGuid().ToString();

        Mock<ITaskManagementConnector> connectorMock = new Mock<ITaskManagementConnector>();
        connectorMock.Setup(c => c.GetDefaultTaskListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(this._anyTaskList);

        connectorMock.Setup(c => c.AddTaskAsync(It.IsAny<string>(), It.IsAny<TaskManagementTask>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(this._anyTask);

        TaskListSkill target = new TaskListSkill(connectorMock.Object);

        // Verify no reminder is set
        Assert.False(this._context.Variables.Get(Parameters.Reminder, out _));

        // Act
        await target.AddTaskAsync(anyTitle, this._context);

        // Assert
        Assert.False(this._context.ErrorOccurred);