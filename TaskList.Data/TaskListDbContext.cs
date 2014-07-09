using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.AspNet.Identity.EntityFramework;
using Misconnex.Model.Models;
using Misconnex.Data.Extensions;
using System.IO;
using TaskList.Model.Models;

namespace TaskList.Data
{
    public class TaskListDbContext : IdentityDbContext<TaskUser>
    {
        public TaskListDbContext()
            : base("DbConnectionString")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<TaskListDbContext>());
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
            this.ReadAllDateTimeValuesAsUtc();
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Project> Projects { get; set; }

        //public DbSet<ProjectCategory> ProjectCategories { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<TaskList.Model.Models.Task> Tasks { get; set; } 
        //public DbSet<TaskUser> Users { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
