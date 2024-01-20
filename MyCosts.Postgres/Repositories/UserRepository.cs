using Microsoft.EntityFrameworkCore;
using MyCosts.Domain.Models;
using MyCosts.Domain.Services;
using MyCosts.Postgres.Entities;
using MyCosts.Postgres.Mapping.Abstractions;
using MyCosts.Postgres.Repositories.Abstractions;

namespace MyCosts.Postgres.Repositories;

public class UserRepository : PostgresRepository<UserEntity, User>, IUserRepository
{
    public UserRepository(PostgresContext postgresContext, IEntityMapper<UserEntity, User> mapper) : base(postgresContext, mapper)
    {
    }

    public async Task<User?> GetAsync(int id)
    {
        var user = await EntitySet.FirstOrDefaultAsync(u => u.Id == id);
        return user == null ? null : Mapper.MapToDomainModel(user);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var user = await EntitySet.FirstOrDefaultAsync(u => u.Email == email);
        return user == null ? null : Mapper.MapToDomainModel(user);
    }
}