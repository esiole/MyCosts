using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
    protected virtual string[]? HardLinkedCollections => null;
    protected virtual string[]? UpdatedCollections => null;

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

    public virtual async Task<TDomainModel?> UpdateAsync(TDomainModel domainModel)
    {
        var updatedEntity = Mapper.MapToEntity(domainModel);

        var entity = await EntitySet.FindAsync(updatedEntity.Id);
        if (entity == null) return null;

        var entityEntry = _postgresContext.Entry(entity);

        entityEntry.CurrentValues.SetValues(updatedEntity);

        if (HardLinkedCollections?.Length > 0)
        {
            foreach (var collectionName in HardLinkedCollections)
            {
                await UpdateHardLinkedCollection(entity, updatedEntity, entityEntry.Collection(collectionName));
            }
        }

        if (UpdatedCollections?.Length > 0)
        {
            foreach (var collectionName in UpdatedCollections)
            {
                await UpdateCollection(entity, updatedEntity, entityEntry.Collection(collectionName));
            }
        }

        await _postgresContext.SaveChangesAsync();

        return Mapper.MapToDomainModel(entity);
    }

    private async Task UpdateCollection(TEntity persistentEntity, TEntity updatedEntity, CollectionEntry collectionEntry)
    {
        var collectionAccessor = collectionEntry.Metadata.GetCollectionAccessor();

        await collectionEntry.LoadAsync();

        var persistentItemDict = collectionEntry.CurrentValue
            .Cast<IPostgresEntity>()
            .ToDictionary(e => e.Id);

        var updatedItems = (IEnumerable<IPostgresEntity>) collectionAccessor.GetOrCreate(updatedEntity, false);

        foreach (var updatedItem in updatedItems)
        {
            if (!persistentItemDict.TryGetValue(updatedItem.Id, out var persistentItem))
            {
                collectionAccessor.Add(persistentEntity, updatedItem, false);
            }
            else
            {
                _postgresContext.Entry(persistentItem).CurrentValues.SetValues(updatedItem);
                persistentItemDict.Remove(updatedItem.Id);
            }
        }

        foreach (var persistentItem in persistentItemDict.Values)
        {
            collectionAccessor.Remove(persistentEntity, persistentItem);
        }
    }

    private async Task UpdateHardLinkedCollection(TEntity persistentEntity, TEntity updatedEntity, CollectionEntry collectionEntry)
    {
        var collectionAccessor = collectionEntry.Metadata.GetCollectionAccessor();

        await collectionEntry.LoadAsync();

        var persistentItems = collectionEntry.CurrentValue.Cast<IPostgresEntity>().ToArray();
        var updatedItems = ((IEnumerable<IPostgresEntity>) collectionAccessor.GetOrCreate(updatedEntity, false)).ToArray();

        foreach (var (persistentItem, updatedItem) in persistentItems.Zip(updatedItems))
        {
            updatedItem.Id = persistentItem.Id;
            _postgresContext.Entry(persistentItem).CurrentValues.SetValues(updatedItem);
        }

        foreach (var newItem in updatedItems.Skip(persistentItems.Length))
        {
            collectionAccessor.Add(persistentEntity, newItem, false);
        }

        foreach (var removedItem in persistentItems.Skip(updatedItems.Count()))
        {
            collectionAccessor.Remove(persistentEntity, removedItem);
        }
    }
}