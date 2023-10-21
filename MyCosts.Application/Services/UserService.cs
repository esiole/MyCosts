using MyCosts.Domain.Models;
using MyCosts.Domain.Services;

namespace MyCosts.Application.Services;

public interface IUserService
{
    Task<User> AddAsync(User user);
    Task<User?> GetAsync(int id);
    Task<User?> GetAsync(string email, string password);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> AddAsync(User user)
    {
        // TODO: Check email duplicate
        // TODO: Encrypt password
        user = await _userRepository.AddAsync(user);
        return user;
    }

    public async Task<User?> GetAsync(int id)
    {
        return await _userRepository.GetAsync(id);
    }

    public async Task<User?> GetAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        // TODO: Encrypted password check
        return user;
    }
}