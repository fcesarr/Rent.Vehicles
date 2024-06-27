using Rent.Vehicles.Entities.Contexts.Interfaces;

namespace Rent.Vehicles.Entities;

public interface IRepository
{
    void SetContext(IDbContext context);
}