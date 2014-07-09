using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using Microsoft.AspNet.Identity;
using TaskList.Data.Interfaces;
using TaskList.Model.Models;
using TaskList.Model.Interfaces;

namespace TaskList.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        public DbContext Context { get; private set; }
        private IRepositoryProvider provider { get; set; }
        
        public UnitOfWork(IRepositoryProvider repositoryProvider)
        {
            Context = CreateContext();
            provider = ConfigureRepositoryProvider(repositoryProvider);
        }

        private DbContext CreateContext()
        {
            var context = new TaskListDbContext();
            context.Configuration.LazyLoadingEnabled = false;
            context.Configuration.ProxyCreationEnabled = false;
            context.Configuration.ValidateOnSaveEnabled = false;
            return context;
        }

        private IRepositoryProvider ConfigureRepositoryProvider(IRepositoryProvider repositoryProvider)
        {
            repositoryProvider.Context = Context;
            return repositoryProvider;
        }

        public void Commit() { Commit(null); }

        public void Commit(string username)
        {
            Context.ChangeTracker.DetectChanges();

            List<DbEntityEntry> created = Context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added).ToList();
            foreach (var entry in created)
            {
                if (entry.Entity is IAudited)
                {
                    IAudited auditable = entry.Entity as IAudited;
                    if (!String.IsNullOrWhiteSpace(username))
                        auditable.CreatedBy = username;
                    auditable.CreationDate = DateTime.UtcNow;
                }
            }

            List<DbEntityEntry> modified = Context.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified).ToList();
            foreach (var entry in modified)
            {
                if (entry.Entity is IAudited)
                {
                    IAudited auditable = entry.Entity as IAudited;
                    if (!String.IsNullOrWhiteSpace(username))
                        auditable.EditedBy = username;
                    auditable.EditDate = DateTime.UtcNow;
                }
            }
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Context != null)
                {
                    Context.Dispose();
                }
            }
        }

        #region IUnitOfWork Members

        public IRepository<Category> Categories { get { return provider.GetRepositoryForType<Category>(); } }
        public IRepository<Project> Projects { get { return provider.GetRepositoryForType<Project>(); } }

        //public IRepository<ProjectCategory> ProjectCategories { get { return provider.GetRepositoryForType<ProjectCategory>(); } }
        public IRepository<ProjectUser> ProjectUsers { get { return provider.GetRepositoryForType<ProjectUser>(); } }
        public IRepository<Task> Tasks { get { return provider.GetRepositoryForType<Task>(); } }
        public IRepository<TaskUser> Users { get { return provider.GetRepositoryForType<TaskUser>(); } }

        public IRepository<TEntity> GetRepositoryForType<TEntity>() where TEntity : class
        {
            return provider.GetRepositoryForType<TEntity>();
        }

        #endregion

        public static UnitOfWork Instantiate()
        {
            return new UnitOfWork(new RepositoryProvider(new RepositoryFactories()));
        }
    }
}
