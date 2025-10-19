using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Index.WebApi.Core;

public static class HttpContextAccessorExtensions
{
    public static Guid? GetUserId(this IHttpContextAccessor httpContextAccessor)
    {
        var guidString = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(guidString, out var guid))
        {
            return guid;
        }

        return null;
    }

    public static bool IsAuthenticated(this IHttpContextAccessor httpContextAccessor)
    {
        return httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}