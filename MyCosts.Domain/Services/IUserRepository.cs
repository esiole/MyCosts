using MyCosts.Domain.Models;

namespace MyCosts.Domain.Services;

public interface IUserRepository
{
    Task<User> AddAsync(User user);
    Task<User?> GetAsync(int id);
    Task<User?> GetByEmailAsync(string email);
}