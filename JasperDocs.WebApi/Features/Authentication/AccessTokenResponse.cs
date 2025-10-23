namespace JasperDocs.WebApi.Features.Authentication;

public class AccessTokenResponse
{
    public required string TokenType { get; set; } = "Bearer";
    public required string AccessToken { get; set; }
    public required long ExpiresIn { get; set; }
    public required string RefreshToken { get; set; }
}
