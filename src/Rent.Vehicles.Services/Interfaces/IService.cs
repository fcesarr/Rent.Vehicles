namespace Rent.Vehicles.Services.Interfaces;

public interface IService<TEntity> :
    ICreateService<TEntity>,
    IDeleteService<TEntity>,
    IUpdateService<TEntity>,
    IGetService<TEntity>,
    IFindService<TEntity>,
    IAction<TEntity> where TEntity : class;

public interface IAction<TEntity> where TEntity : class;