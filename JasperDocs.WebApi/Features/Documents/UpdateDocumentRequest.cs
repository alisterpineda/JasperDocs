namespace JasperDocs.WebApi.Features.Documents;

public class UpdateDocumentRequest
{
    public required string Title { get; init; }
    public string? Description { get; init; }
}
