namespace TaskList.Data.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using TaskList.Model.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<TaskList.Data.TaskListDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TaskList.Data.TaskListDbContext context)
        {
            //context.Airports.AddOrUpdate<Airport>(
            //    a => a.Code,
            //    new Airport
            //    {
            //    Code = "LHR",
            //    HasTerminals = true,
            //    Name = "Heathrow Airport",
            //    CreatedBy = "Seeded",
            //    CreationDate = DateTime.Now,
            //    Terminals = new List<Terminal>()
            //        {
            //            new Terminal
            //            {
            //                Code = "T1",
            //                Name = "Terminal 1",
            //                CreatedBy = "Seeded",
            //                CreationDate = DateTime.Now,
            //            },
            //            new Terminal
            //            {
            //                Code = "T2",
            //                Name = "Terminal 2",
            //                CreatedBy = "Seeded",
            //                CreationDate = DateTime.Now,
            //            },
            //            new Terminal
            //            {
            //                Code = "T3",
            //                Name = "Terminal 3",
            //                CreatedBy = "Seeded",
            //                CreationDate = DateTime.Now,
            //            },
            //            new Terminal
            //            {
            //                Code = "T4",
            //                Name = "Terminal 4",
            //                CreatedBy = "Seeded",
            //                CreationDate = DateTime.Now,
            //            },
            //            new Terminal
            //            {
            //                Code = "T5",
            //                Name = "Terminal 5",
            //                CreatedBy = "Seeded",
            //                CreationDate = DateTime.Now,
            //            }
            //        }
            //    },
            //    new Airport
            //    {
            //        Code = "LGW",
            //        HasTerminals = true,
            //        Name = "Gatwick Airport",
            //        CreatedBy = "Seeded",
            //        CreationDate = DateTime.Now,
            //        Terminals = new List<Terminal>()
            //        {
            //            new Terminal
            //            {
            //                Code = "N",
            //                Name = "North Terminal",
            //                CreatedBy = "Seeded",
            //                CreationDate = DateTime.Now,
            //            },
            //            new Terminal
            //            {
            //                Code = "S",
            //                Name = "South Terminal",
            //                CreatedBy = "Seeded",
            //                CreationDate = DateTime.Now,
            //            }
            //        }
            //    },
            //    new Airport
            //    {
            //        Code = "LCY",
            //        HasTerminals = true,
            //        Name = "London City Airport",
            //        CreatedBy = "Seeded",
            //        CreationDate = DateTime.Now,
            //        Terminals = new List<Terminal>()
            //        {
            //            new Terminal
            //            {
            //                Code = "M",
            //                Name = "Main Terminal",
            //                CreatedBy = "Seeded",
            //                CreationDate = DateTime.Now,
            //            }
            //        }
            //    });

            //context.SaveChanges();

            //context.Airlines.AddOrUpdate<Airline>(
            //    a => a.Code,
            //    new Airline
            //    {
            //        Code = "BA",
            //        Name = "British Airways",
            //        Address = "112 Bath Road, Heaton Chapel, Bury, TW19 7JE",
            //        BillingAddress = "112 Bath Road, Heaton Chapel, Bury, TW19 7JE",
            //        CreatedBy = "Seeded",
            //        CreationDate = DateTime.Now,
            //        Type = Model.Enums.CompanyType.Airline,
            //        Employees = new List<Employee> 
            //        { 
            //            new Employee
            //            {
            //                Name = "Mike Hesselntine",
            //                Title = "Manager",
            //            }
            //        }
            //    },
            //    new Airline
            //    {
            //        Code = "AZ",
            //        Name = "Alitalia",
            //        Address = "112 Bath Road, Heaton Chapel, Bury, TW19 7JE",
            //        BillingAddress = "112 Bath Road, Heaton Chapel, Bury, TW19 7JE",
            //        CreatedBy = "Seeded",
            //        CreationDate = DateTime.Now,
            //        Type = Model.Enums.CompanyType.Airline,
            //        Employees = new List<Employee> 
            //        { 
            //            new Employee
            //            {
            //                Name = "Jonathan Pierce",
            //                Title = "Manager",
            //            }
            //        }
            //    }
            //);

            //context.SaveChanges();

            context.Categories.AddOrUpdate<Category>(
                c => c.ID,
                new Category
                {
                    ID = 1,
                    Name = "Category 1",
                    CreatedBy = "Seeded",
                    CreationDate = DateTime.Now,
                },
                new Category
                {
                    ID = 2,
                    Name = "Category 2",
                    CreatedBy = "Seeded",
                    CreationDate = DateTime.Now,
                },
                new Category
                {
                    ID = 3,
                    Name = "Category 3",
                    CreatedBy = "Seeded",
                    CreationDate = DateTime.Now,
                }
            );

            context.SaveChanges();
        }
    }
}
