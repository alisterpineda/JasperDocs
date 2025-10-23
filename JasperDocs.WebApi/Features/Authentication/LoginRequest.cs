using System.ComponentModel.DataAnnotations;

namespace JasperDocs.WebApi.Features.Authentication;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

    public string? TwoFactorCode { get; set; }

    public string? TwoFactorRecoveryCode { get; set; }
}
