using MyCosts.Api.Models.Auth;
using MyCosts.Domain.Models;

namespace MyCosts.Api.Mapping;

public static class UserMapper
{
    public static User ToNewUser(this LoginModel loginModel) => new()
    {
        Id = default,
        Email = loginModel.Email,
        Password = loginModel.Password,
    };
}