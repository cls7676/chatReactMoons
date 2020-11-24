// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.Memory.Storage;

namespace Microsoft.SemanticKernel.Memory;

/// <summary>
/// Implementation of <see cref="ISemanticTextMemory"/>./>.
/// </summary>
public sealed class SemanticTextMemory : ISemanticTextMemory, IDisposable
{
    private readonly IEmbeddingGenerator<string, float> _embeddingGenerator;
    private readonly IMemoryStore<float> _storage;

    public Sem