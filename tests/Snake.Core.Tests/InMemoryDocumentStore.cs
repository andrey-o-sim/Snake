namespace Snake.Core.Tests;

internal sealed class InMemoryDocumentStore : IDocumentStore
{
    private readonly Dictionary<StoredDocument, string> documents = [];

    public bool Exists(StoredDocument document) => documents.ContainsKey(document);

    public string? Read(StoredDocument document) => documents.GetValueOrDefault(document);

    public void Write(StoredDocument document, string content) => documents[document] = content;

    internal string Content(StoredDocument document) => documents[document];
}
