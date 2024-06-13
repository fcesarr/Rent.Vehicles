using MongoDB.Driver;
using MongoDB.Driver.Linq;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Repositories.Interfaces;

namespace Rent.Vehicles.Services.Repositories;

public sealed class MongoRepository<T> : IMongoRepository<T> where T : Entity
{
    private readonly IMongoCollection<T> _mongoCollection;

    public MongoRepository(IMongoDatabase mongoDatabase)
    {
        _mongoCollection = mongoDatabase
            .GetCollection<T>($"{typeof(T).Name.ToLower()}s");
    }

    public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var options = new InsertOneOptions();

        await _mongoCollection.InsertOneAsync(entity, options, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<T>
            .Filter
            .Eq(t => t.Id, id);
    
        await _mongoCollection.DeleteOneAsync(filter, cancellationToken);
    }

    public async Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _mongoCollection
            .AsQueryable()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var options = new UpdateOptions();

        var filter = Builders<T>
            .Filter
            .Eq(r => r.Id, entity.Id);
        
        var update = Builders<T>.Update
            .Set(t => t, entity);
    
        await _mongoCollection.UpdateOneAsync(filter, update, options, cancellationToken);
    }
}