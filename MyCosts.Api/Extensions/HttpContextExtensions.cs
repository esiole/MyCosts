using MyCosts.Domain.Models;

namespace MyCosts.Api.Extensions;

public static class HttpContextExtensions
{
    public static User GetUser(this HttpContext context) => (context.Items["User"] as User)!;
}