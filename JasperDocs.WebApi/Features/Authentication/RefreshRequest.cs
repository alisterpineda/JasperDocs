using System.ComponentModel.DataAnnotations;

namespace JasperDocs.WebApi.Features.Authentication;

public class RefreshRequest
{
    [Required]
    public required string RefreshToken { get; set; }
}
