using MyCosts.Postgres.Entities.Abstractions;

namespace MyCosts.Postgres.Mapping.Abstractions;

public interface IEntityMapper<TEntity, TDomainModel>
    where TEntity : class, IPostgresEntity
    where TDomainModel : class
{
    TEntity MapToEntity(TDomainModel domainModel);
    TDomainModel MapToDomainModel(TEntity entity);
}