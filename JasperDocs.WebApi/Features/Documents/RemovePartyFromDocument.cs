namespace JasperDocs.WebApi.Features.Documents;

public class RemovePartyFromDocument
{
    public required Guid DocumentId { get; init; }
    public required Guid PartyId { get; init; }
}
