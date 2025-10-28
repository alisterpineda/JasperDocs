namespace JasperDocs.WebApi.Features.Documents;

public class UpdateDocument
{
    public required Guid DocumentId { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required List<Guid> PartyIds { get; init; }
}
