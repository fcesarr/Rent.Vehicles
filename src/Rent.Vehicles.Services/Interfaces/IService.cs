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

public interface ISqlService<TEntity> : IService<TEntity> where TEntity : Entity;

public interface INoSqlService<TEntity> : IService<TEntity> where TEntity : Entity;