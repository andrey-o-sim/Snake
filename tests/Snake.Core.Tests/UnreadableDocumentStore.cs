namespace Snake.Core.Tests;

internal sealed class UnreadableDocumentStore : IDocumentStore
{
    public bool Exists(StoredDocument document) => true;

    public string? Read(StoredDocument document) => throw new IOException("The document cannot be read.");

    public void Write(StoredDocument document, string content) =>
        throw new IOException("The document cannot be written.");
}
