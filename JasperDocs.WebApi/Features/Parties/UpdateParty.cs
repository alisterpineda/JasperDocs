namespace JasperDocs.WebApi.Features.Parties;

public class UpdateParty
{
    public required Guid PartyId { get; init; }
    public required string Name { get; init; }
}
