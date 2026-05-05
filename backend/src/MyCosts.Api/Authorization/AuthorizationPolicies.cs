namespace MyCosts.Api.Authorization;

public static class AuthSchemes
{
    public const string Cookie = "Cookie";
    public const string Bearer = "Bearer";
}

public static class AuthorizationPolicies
{
    public const string CookieOnly = "CookieOnly";
    public const string BearerOnly = "BearerOnly";
}
