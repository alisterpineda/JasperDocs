using System.ComponentModel.DataAnnotations;

namespace JasperDocs.WebApi.Features.Authentication;

public class LoginRequest
{
    [Required]
    public required string Username { get; set; }

    [Required]
    public required string Password { get; set; }

    public string? TwoFactorCode { get; set; }

    public string? TwoFactorRecoveryCode { get; set; }
}
