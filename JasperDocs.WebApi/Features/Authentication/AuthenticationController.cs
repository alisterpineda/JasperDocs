using JasperDocs.WebApi.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JasperDocs.WebApi.Features.Authentication;

[ApiController]
[Route("")]
public class AuthenticationController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AccessTokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IResult> LoginAsync(
        [FromServices] IRequestHandler<LoginRequest, Microsoft.AspNetCore.Identity.SignInResult> handler,
        [FromBody] LoginRequest request,
        CancellationToken ct = default)
    {
        var result = await handler.HandleAsync(request, ct);

        if (!result.Succeeded)
        {
            return TypedResults.Problem(
                result.ToString(),
                statusCode: StatusCodes.Status401Unauthorized);
        }

        // Return Empty - SignInManager automatically writes the bearer token response
        return TypedResults.Empty;
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType<AccessTokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IResult> RefreshAsync(
        [FromServices] IRequestHandler<RefreshRequest, IResult> handler,
        [FromBody] RefreshRequest request,
        CancellationToken ct = default)
    {
        return await handler.HandleAsync(request, ct);
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IResult> LogoutAsync(
        [FromServices] IRequestHandler<LogoutRequest> handler,
        [FromBody] LogoutRequest request,
        CancellationToken ct = default)
    {
        if (request == null)
        {
            return TypedResults.Unauthorized();
        }

        await handler.HandleAsync(request, ct);
        return TypedResults.Ok();
    }
}
