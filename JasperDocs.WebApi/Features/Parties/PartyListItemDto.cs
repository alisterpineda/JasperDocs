namespace JasperDocs.WebApi.Features.Parties;

public class PartyListItemDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required DateTime CreatedAt { get; init; }
}
