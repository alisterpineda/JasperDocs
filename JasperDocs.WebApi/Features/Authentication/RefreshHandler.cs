using System.Security.Claims;
using JasperDocs.WebApi.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace JasperDocs.WebApi.Features.Authentication;

public class RefreshHandler : IRequestHandler<RefreshRequest, IResult>
{
    public Task<IResult> HandleAsync(RefreshRequest request, CancellationToken ct = default)
    {
        // Create a minimal principal with BearerScheme identity
        // The BearerTokenHandler will validate the refresh token and recreate the full principal
        var refreshPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(IdentityConstants.BearerScheme));

        // Set the refresh token in authentication properties
        var properties = new AuthenticationProperties();
        properties.Items[".refresh_token"] = request.RefreshToken;

        // SignIn will trigger the BearerTokenHandler which validates the refresh token
        // and returns a new AccessTokenResponse with new access and refresh tokens
        IResult result = TypedResults.SignIn(
            refreshPrincipal,
            properties,
            IdentityConstants.BearerScheme);

        return Task.FromResult(result);
    }
}
