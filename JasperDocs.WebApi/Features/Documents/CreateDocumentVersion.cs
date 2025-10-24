namespace JasperDocs.WebApi.Features.Documents;

public class CreateDocumentVersion
{
    public required Guid DocumentId { get; set; }
    public string? Description { get; set; }
    public required IFormFile File { get; set; }
}
