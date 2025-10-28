using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Core.Exceptions;
using JasperDocs.WebApi.Entities;
using Microsoft.AspNetCore.Identity;

namespace JasperDocs.WebApi.Features.Authentication;

public class GetCurrentUserInfoHandler : IRequestHandler<GetCurrentUserInfo, GetCurrentUserInfoResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetCurrentUserInfoHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task<GetCurrentUserInfoResponse> HandleAsync(
        GetCurrentUserInfo request,
        CancellationToken ct = default)
    {
        var userId = _httpContextAccessor.GetUserId();
        if (!userId.HasValue)
        {
            throw new NotFoundException("User not found.");
        }

        var user = await _userManager.FindByIdAsync(userId.Value.ToString());
        if (user == null)
        {
            throw new NotFoundException("User", userId.Value);
        }

        return new GetCurrentUserInfoResponse
        {
            Id = user.Id,
            Username = user.UserName!
        };
    }
}
