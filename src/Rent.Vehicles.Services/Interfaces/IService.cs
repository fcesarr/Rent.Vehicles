using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface IService<TEntity> :
    ICreateService<TEntity>,
    IDeleteService<TEntity>,
    IUpdateService<TEntity>,
    IGetService<TEntity>,
    IFindService<TEntity> where TEntity : Entity
{

}

public interface IBothServices<TEntity> : IService<TEntity> where TEntity : Entity;