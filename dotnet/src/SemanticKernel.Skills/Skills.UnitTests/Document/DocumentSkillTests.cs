// Copyright (c) Microsoft. All rights reserved.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Skills.Document;
using Microsoft.SemanticKernel.Skills.Document.FileSystem;
using Moq;
using Xunit;
using static Microsoft.SemanticKernel.Skills.Document.DocumentSkill;

namespace SemanticKernel.Skills.UnitTests.Document;

public class DocumentSkillTests
{
    private readonly SKContext _context = new(new ContextVariables(), NullMemory.Instance, null, NullLogger.Instance);

    [Fact]
    public async Task ReadTextAsyncSucceedsAsync()
    {
        // Arrange
        var expectedText = Guid.NewGuid().ToString();
        var anyFilePath = Guid.NewGuid().ToString();

        var fileSystemConnectorMock = new Mock<IFileSystemConnector>();
        fileSystemConnectorMock
            .Setup(mock => mock.GetFileContentStreamAsync(It.Is<string>(filePath => filePath.Equals(anyFilePath, StringComparison.Ordinal)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Stream.N