using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Entities;
using Microsoft.AspNetCore.Identity;

namespace JasperDocs.WebApi.Features.Authentication;

public class LogoutHandler : IRequestHandler<LogoutRequest>
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LogoutHandler(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task HandleAsync(LogoutRequest request, CancellationToken ct = default)
    {
        await _signInManager.SignOutAsync();
    }
}
