using Microsoft.EntityFrameworkCore;
using MyCosts.Postgres.Entities.Abstractions;
using MyCosts.Postgres.Mapping.Abstractions;

namespace MyCosts.Postgres.Repositories.Abstractions;

public abstract class PostgresRepository<TEntity, TDomainModel>
    where TEntity : class, IPostgresEntity
    where TDomainModel : class
{
    private readonly PostgresContext _postgresContext;
    protected readonly IEntityMapper<TEntity, TDomainModel> Mapper;

    protected DbSet<TEntity> EntitySet => _postgresContext.Set<TEntity>();

    protected PostgresRepository(PostgresContext postgresContext, IEntityMapper<TEntity, TDomainModel> mapper)
    {
        _postgresContext = postgresContext;
        Mapper = mapper;
    }

    public async Task<TDomainModel> AddAsync(TDomainModel domainModel)
    {
        var entity = Mapper.MapToEntity(domainModel);
        EntitySet.Add(entity);
        await _postgresContext.SaveChangesAsync();
        return Mapper.MapToDomainModel(entity);
    }

    public async Task DeleteAsync(int id)
    {
        await _postgresContext.Set<TEntity>().Where(e => e.Id == id).ExecuteDeleteAsync();
    }

    public async Task<TDomainModel?> UpdateAsync(TDomainModel domainModel)
    {
        var updatedEntity = Mapper.MapToEntity(domainModel);

        var entity = await EntitySet.FindAsync(updatedEntity.Id);
        if (entity == null) return null;

        _postgresContext.Entry(entity).CurrentValues.SetValues(updatedEntity);
        await _postgresContext.SaveChangesAsync();

        return Mapper.MapToDomainModel(entity);
    }
}