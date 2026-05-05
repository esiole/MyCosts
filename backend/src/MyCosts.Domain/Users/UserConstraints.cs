namespace MyCosts.Domain.Users;

public static class UserConstraints
{
    public const int PasswordMinLength = 8;
    public const int PasswordMaxLength = 128;
    public const int EmailMaxLength = 256;
}
