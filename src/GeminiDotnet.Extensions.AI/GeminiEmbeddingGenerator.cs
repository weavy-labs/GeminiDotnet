﻿using GeminiDotnet.Embeddings;

using Microsoft.Extensions.AI;

namespace GeminiDotnet.Extensions.AI;

/// <summary>
/// An <see cref="IChatClient"/> implementation for the Gemini AI service.
/// </summary>
public sealed class GeminiEmbeddingGenerator : IEmbeddingGenerator<string, Embedding<float>>
{
    private readonly GeminiClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeminiEmbeddingGenerator"/> class.
    /// </summary>
    /// <param name="options">The options to use for the client.</param>
    public GeminiEmbeddingGenerator(GeminiClientOptions options) : this(new GeminiClient(options))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeminiEmbeddingGenerator"/> class.
    /// </summary>
    /// <param name="client">The <see cref="GeminiClient"/> to use.</param>
    public GeminiEmbeddingGenerator(GeminiClient client)
    {
        ArgumentNullException.ThrowIfNull(client);

        _client = client;
        Metadata = new EmbeddingGeneratorMetadata("Gemini", client.Endpoint);
    }

    public EmbeddingGeneratorMetadata Metadata { get; }

    public async Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(
        IEnumerable<string> values,
        EmbeddingGenerationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (options?.ModelId is null)
        {
            throw new ArgumentException($"The {nameof(options.ModelId)} property must be set", nameof(options));
        }

        var request = ExtensionsAIToGeminiMapper.CreateMappedEmbeddingRequest(values);
        var response = await _client.EmbedContentAsync(options.ModelId, request, cancellationToken).ConfigureAwait(false);
        return GeminiToExtensionsAIMapper.CreateMappedGeneratedEmbeddings(response);
    }


    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        if (serviceType == typeof(GeminiClient))
        {
            return _client;
        }

        return null;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}