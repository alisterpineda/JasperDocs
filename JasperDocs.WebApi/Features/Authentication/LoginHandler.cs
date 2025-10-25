using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Entities;
using Microsoft.AspNetCore.Identity;

namespace JasperDocs.WebApi.Features.Authentication;

public class LoginHandler : IRequestHandler<LoginRequest, SignInResult>
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LoginHandler(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<SignInResult> HandleAsync(LoginRequest request, CancellationToken ct = default)
    {
        // Default to bearer token authentication (not cookies)
        _signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;

        var result = await _signInManager.PasswordSignInAsync(
            request.Username,
            request.Password,
            isPersistent: false,
            lockoutOnFailure: true);

        // Handle two-factor authentication if required
        if (result.RequiresTwoFactor)
        {
            if (!string.IsNullOrEmpty(request.TwoFactorCode))
            {
                result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
                    request.TwoFactorCode,
                    isPersistent: false,
                    rememberClient: false);
            }
            else if (!string.IsNullOrEmpty(request.TwoFactorRecoveryCode))
            {
                result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(
                    request.TwoFactorRecoveryCode);
            }
        }

        return result;
    }
}
