using MyCosts.Domain.Models;
using MyCosts.Postgres.Entities;
using MyCosts.Postgres.Mapping.Abstractions;

namespace MyCosts.Postgres.Mapping;

internal class UserMapper : IEntityMapper<UserEntity, User>
{
    public UserEntity MapToEntity(User domainModel) => new()
    {
        Id = domainModel.Id,
        Email = domainModel.Email,
        Password = domainModel.Password,
    };

    public User MapToDomainModel(UserEntity entity) => new()
    {
        Id = entity.Id,
        Email = entity.Email,
        Password = entity.Password,
    };
}