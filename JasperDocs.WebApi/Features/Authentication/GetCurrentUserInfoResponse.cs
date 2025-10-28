namespace JasperDocs.WebApi.Features.Authentication;

public class GetCurrentUserInfoResponse
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
}
