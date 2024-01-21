namespace MyCosts.Api.Models.Auth;

/// <summary>
///     Application authentication data
/// </summary>
public class LoginModel
{
    /// <summary>
    ///     User email
    /// </summary>
    /// <example>testtesttest@gmail.com</example>
    public required string Email { get; set; }

    /// <summary>
    ///     User password
    /// </summary>
    /// <example>password</example>
    public required string Password { get; set; }
}