using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using TaskList.Model.Models;

namespace TaskList.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        void Commit(string username);
        
        DbContext Context { get; }

        // Repositories
        IRepository<Category> Categories { get; }
        IRepository<Project> Projects { get; }
        //IRepository<ProjectCategory> ProjectCategories { get; }
        IRepository<ProjectUser> ProjectUsers { get; } 
        IRepository<Task> Tasks { get; } 
        IRepository<TaskUser> Users { get; } 

        IRepository<TEntity> GetRepositoryForType<TEntity>() where TEntity : class;
    }
}
