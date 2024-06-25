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

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
        bool descending = false,
        Expression<Func<TEntity, dynamic>>? orderBy = default,
        IEnumerable<Expression<Func<TEntity, dynamic>>>? includes = default,
        CancellationToken cancellationToken = default)
    {
        var findOptions = new FindOptions<TEntity>();

        if(orderBy is not null)
        {
            var sortDefinition = Builders<TEntity>.Sort;

            var sort = sortDefinition.Ascending(orderBy);

            if(descending)
                sort = sortDefinition.Descending(orderBy);

            findOptions.Sort = sort;
        }

        if(includes is not null)
        {
            var projectionDefinition = Builders<TEntity>.Projection;

            var projection = projectionDefinition.Combine(includes.Select(include => projectionDefinition.Include(include)));

            findOptions.Projection = projection;
        }

        var filter = Builders<TEntity>.Filter
            .Where(predicate);
        
        var cursor =  await _mongoCollection
            .FindAsync(filter, options: findOptions, cancellationToken: cancellationToken);

        return await cursor
            .ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate,
        bool descending = false,
        Expression<Func<TEntity, dynamic>>? orderBy = default,
        IEnumerable<Expression<Func<TEntity, dynamic>>>? includes = default,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var findOptions = new FindOptions<TEntity>();

            if(orderBy is not null)
            {
                var sortDefinition = Builders<TEntity>.Sort;

                var sort = sortDefinition.Ascending(orderBy);

                if(descending)
                    sort = sortDefinition.Descending(orderBy);

                findOptions.Sort = sort;
            }

            if(includes is not null)
            {
                var projectionDefinition = Builders<TEntity>.Projection;

                var projection = projectionDefinition.Combine(includes.Select(include => projectionDefinition.Include(include)));

                findOptions.Projection = projection;
            }
            
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