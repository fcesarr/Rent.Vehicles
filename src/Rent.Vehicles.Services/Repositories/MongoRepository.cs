using System.Linq.Expressions;

using Microsoft.Extensions.Logging;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Repositories.Interfaces;

namespace Rent.Vehicles.Services.Repositories;

public sealed class MongoRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    private readonly ILogger<MongoRepository<TEntity>> _logger;

    private readonly IMongoCollection<TEntity> _mongoCollection;

    public MongoRepository(ILogger<MongoRepository<TEntity>> logger,
        IMongoDatabase mongoDatabase)
    {
        _logger = logger;
        _mongoCollection = mongoDatabase
            .GetCollection<TEntity>($"{typeof(TEntity).Name.ToLower()}s");
    }

    public async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var options = new InsertOneOptions();

        await _mongoCollection.InsertOneAsync(entity, options, cancellationToken);
    }

    public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>
            .Filter.Where(predicate);
    
        _ = await _mongoCollection.DeleteManyAsync(filter, cancellationToken);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>
            .Filter
            .Where(x => x.Id == entity.Id);

        _ = await _mongoCollection.DeleteOneAsync(filter, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter
            .Where(predicate);
        
        var cursor =  await _mongoCollection
            .FindAsync(filter, cancellationToken: cancellationToken);

        return await cursor
            .ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            
            var filter = Builders<TEntity>.Filter
                .Where(predicate);
            
            var cursor =  await _mongoCollection
                .FindAsync(filter, cancellationToken: cancellationToken);
    
            return await cursor
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            
            _logger.LogError(ex.Message, ex);
        }

        return await Task.FromResult(default(TEntity));
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var options = new UpdateOptions();

        var filter = Builders<TEntity>
            .Filter
            .Where(x => x.Id == entity.Id);
    
        await Task.Run(async () => await _mongoCollection.ReplaceOneAsync(filter, entity) , cancellationToken);
    }
}