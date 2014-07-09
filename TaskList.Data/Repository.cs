using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using TaskList.Data.Interfaces;

namespace TaskList.Data
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DbContext context { get; private set; }
        public DbSet<TEntity> Set { get; private set; }

        public Repository(DbContext _context)
        {
            if (_context == null)
                throw new ArgumentNullException("context");

            context = _context;
            Set = _context.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> Get()
        {
            return Set;
        }

        public virtual TEntity Find(object id)
        {
            return Set.Find(id);
        }

        public virtual void Add(TEntity entity)
        {
            DbEntityEntry entry = context.Entry(entity);
            if (entry.State != EntityState.Detached)
            {
                entry.State = EntityState.Added;
            }
            else
            {
                Set.Add(entity);
            }
        }

        public virtual void Update(TEntity entity)
        {
            DbEntityEntry entry = context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                Set.Attach(entity);
            }
            entry.State = EntityState.Modified;
        }

        public virtual void Delete(TEntity entity)
        {
            DbEntityEntry entry = context.Entry(entity);
            if (entry.State != EntityState.Deleted)
            {
                entry.State = EntityState.Deleted;
            }
            else
            {
                Set.Attach(entity);
                Set.Remove(entity);
            }
        }

        public virtual void Delete(object id)
        {
            var entity = Find(id);
            if (entity == null) return;
            Delete(entity);
        }

        public virtual bool Exists(object id)
        {
            return (Find(id) != null);
        }
    }
}
