namespace JasperDocs.WebApi.Features.Parties;

public class GetPartyResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
