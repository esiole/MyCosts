using MyCosts.Domain.Models;
using MyCosts.Postgres.Entities;

namespace MyCosts.Postgres.Mapping;

public static class UserMapper
{
    public static UserEntity ToEntity(this User domainModel) => new()
    {
        Id = domainModel.Id,
        Email = domainModel.Email,
        Password = domainModel.Password,
    };

    public static User ToDomainModel(this UserEntity entity) => new()
    {
        Id = entity.Id,
        Email = entity.Email,
        Password = entity.Password,
    };
}