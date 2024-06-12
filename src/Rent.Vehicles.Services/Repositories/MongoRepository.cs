using MongoDB.Driver;

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

}