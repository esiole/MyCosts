using System.Security.Claims;

namespace MyCosts.Api.Authorization;

internal static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal user)
    {
        public Guid GetUserId()
        {
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new InvalidOperationException("User ID claim is missing.");

            return Guid.Parse(value);
        }
    }
}
