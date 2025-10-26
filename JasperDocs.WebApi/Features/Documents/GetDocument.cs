namespace JasperDocs.WebApi.Features.Documents;

public class GetDocument
{
    public required Guid DocumentId { get; init; }
    public int? VersionNumber { get; init; }
}
