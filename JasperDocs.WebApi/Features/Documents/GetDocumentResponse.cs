namespace JasperDocs.WebApi.Features.Documents;

public class GetDocumentResponse
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required DocumentVersionDto SelectedVersion { get; init; }
    public required IReadOnlyList<DocumentVersionDto> AvailableVersions { get; init; }
}

public class DocumentVersionDto
{
    public required Guid Id { get; init; }
    public required int VersionNumber { get; init; }
    public string? Description { get; init; }
    public required string MimeType { get; init; }
    public required DateTime CreatedAt { get; init; }
}
