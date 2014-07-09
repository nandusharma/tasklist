using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskList.Data.Interfaces;

namespace TaskList.Data
{
    public class RepositoryProvider : IRepositoryProvider
    {
        public DbContext Context { get; set; }
        protected IRepositoryFactories factories { get; private set;}
        protected Dictionary<Type, object> Repositories { get; private set; }

        public RepositoryProvider(IRepositoryFactories repositoryFactories)
        {
            factories = repositoryFactories;
            Repositories = new Dictionary<Type, object>();
        }

        public IRepository<TEntity> GetRepositoryForType<TEntity>() where TEntity : class
        {
            return GetRepository<IRepository<TEntity>>(factories.GetRepositoryFactoryForEntityType<TEntity>());
        }

        public TEntity GetRepository<TEntity>(Func<System.Data.Entity.DbContext, object> factory = null) where TEntity : class
        {
            object repoObj;
            Repositories.TryGetValue(typeof(TEntity), out repoObj);
            if (repoObj != null)
            {
                return (TEntity)repoObj;
            }

            // Not found or null; make one, add to dictionary cache, and return it.
            return MakeRepository<TEntity>(factory, Context);
        }

        protected virtual TEntity MakeRepository<TEntity>(Func<DbContext, object> factory, DbContext dbContext)
        {
            var f = factory ?? factories.GetRepositoryFactory<TEntity>();
            if (f == null)
            {
                throw new NotImplementedException("No factory for repository type requested.");
            }
            var repo = (TEntity)f(dbContext);
            SetRepository(repo);
            return repo;
        }

        public void SetRepository<TEntity>(TEntity repository)
        {
            Repositories[typeof(TEntity)] = repository;
        }
    }
}
