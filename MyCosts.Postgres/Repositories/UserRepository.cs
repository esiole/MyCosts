using Microsoft.EntityFrameworkCore;
using MyCosts.Domain.Models;
using MyCosts.Domain.Services;
using MyCosts.Postgres.Mapping;

namespace MyCosts.Postgres.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PostgresContext _postgresContext;

    public UserRepository(PostgresContext postgresContext)
    {
        _postgresContext = postgresContext;
    }

    public async Task<User> AddAsync(User user)
    {
        var userEntity = _postgresContext.Users.Add(user.ToEntity()).Entity;
        await _postgresContext.SaveChangesAsync();
        return userEntity.ToDomainModel();
    }

    public async Task<User?> GetAsync(int id)
    {
        var user = await _postgresContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        return user?.ToDomainModel();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var user = await _postgresContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user?.ToDomainModel();
    }
}