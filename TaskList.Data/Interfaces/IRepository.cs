using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskList.Data.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        DbSet<TEntity> Set { get; }
        IQueryable<TEntity> Get();
        TEntity Find(object id);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        void Delete(object id);
        bool Exists(object id);
    }
}
