namespace JasperDocs.WebApi.Features.Documents;

public class DocumentListItemDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required DateTime CreatedAt { get; init; }
}
