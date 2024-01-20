using MyCosts.Domain.Models;
using MyCosts.Domain.Services;

namespace MyCosts.Application.Services;

public interface IUserService
{
    Task<User> AddAsync(User user);
    Task<User?> GetAsync(int id);
    Task<User?> GetAsync(string email, string password);
}

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<User> AddAsync(User user)
    {
        // TODO: Check email duplicate
        // TODO: Encrypt password
        user = await userRepository.AddAsync(user);
        return user;
    }

    public async Task<User?> GetAsync(int id)
    {
        return await userRepository.GetAsync(id);
    }

    public async Task<User?> GetAsync(string email, string password)
    {
        var user = await userRepository.GetByEmailAsync(email);

        // TODO: Encrypted password check
        return user;
    }
}