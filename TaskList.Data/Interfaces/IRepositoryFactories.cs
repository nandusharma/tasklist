using System;

namespace TaskList.Data.Interfaces
{
    public interface IRepositoryFactories
    {
        Func<System.Data.Entity.DbContext, object> GetRepositoryFactory<TEntity>();
        Func<System.Data.Entity.DbContext, object> GetRepositoryFactoryForEntityType<TEntity>() where TEntity : class;
    }
}
