using System;
using System.Data.Entity;

namespace TaskList.Data.Interfaces
{
    public interface IRepositoryProvider
    {
        DbContext Context { get; set; }
        IRepository<TEntity> GetRepositoryForType<TEntity>() where TEntity : class;
        TEntity GetRepository<TEntity>(Func<DbContext, object> factory = null) where TEntity : class;
        void SetRepository<TEntity>(TEntity repository);
    }
}
