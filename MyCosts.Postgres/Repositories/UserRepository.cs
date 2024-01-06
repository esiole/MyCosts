using Microsoft.EntityFrameworkCore;
using MyCosts.Domain.Models;
using MyCosts.Domain.Services;
using MyCosts.Postgres.Entities;
using MyCosts.Postgres.Mapping.Abstractions;

namespace MyCosts.Postgres.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PostgresContext _postgresContext;
    private readonly IEntityMapper<UserEntity, User> _mapper;

    public UserRepository(PostgresContext postgresContext, IEntityMapper<UserEntity, User> mapper)
    {
        _postgresContext = postgresContext;
        _mapper = mapper;
    }

    public async Task<User> AddAsync(User user)
    {
        var userEntity = _postgresContext.Users.Add(_mapper.MapToEntity(user)).Entity;
        await _postgresContext.SaveChangesAsync();
        return _mapper.MapToDomainModel(userEntity);
    }

    public async Task<User?> GetAsync(int id)
    {
        var user = await _postgresContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        return user == null ? null : _mapper.MapToDomainModel(user);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var user = await _postgresContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user == null ? null : _mapper.MapToDomainModel(user);
    }
}