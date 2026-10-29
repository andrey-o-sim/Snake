namespace Snake.Core;

public interface IDocumentStore
{
    bool Exists(StoredDocument document);

    string? Read(StoredDocument document);

    void Write(StoredDocument document, string content);
}
