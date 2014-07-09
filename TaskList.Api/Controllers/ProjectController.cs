using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TaskList.Api.Filters;
using TaskList.Api.Models;
using TaskList.Model.Models;

namespace TaskList.Api.Controllers
{
    [RoutePrefix("api/Project")]
    public class ProjectController : BaseController<Project>
    {
        public ProjectController() : base() {}

        [Queryable, HttpGet, Route("GetProjects")]
        public IHttpActionResult GetProjectsByUser(string userID)
        {
            var projects = Uow.Projects.Set.Include("Tasks").Where(p => (
                p.User.Id == userID
            )).Select(p => new {
                ID = p.ID,
                Name = p.Name,
                IsDefault = p.IsDefault,
                IsShared = p.IsShared,
                Order = p.Order,                
                Tasks = (ICollection<Task>)p.Tasks.Where(t => t.User.Id == userID)
            }).OrderBy(p => p.Order).ToList();

            return Ok(projects.Select(p => new { ID = p.ID, Name = p.Name, IsDefault = p.IsDefault, IsShared = p.IsShared, Order = p.Order, TaskCount = p.Tasks.Count, IsNew = false, IsEdited = false }));
        }

        [HttpPost]
        public override IHttpActionResult Post(Project project)
        {
            if (!string.IsNullOrWhiteSpace(project.Name) && !string.IsNullOrWhiteSpace(project.UserID))
            {
                var user = Uow.Users.Set.Where(u => u.Id == project.UserID).FirstOrDefault();

                if (user != null)
                {
                    var newProject = new Project()
                    {
                        ID = 0,
                        Name = project.Name,
                        UserID = project.UserID,
                        CreatedBy = project.UserID,
                        CreationDate = DateTime.Now,
                    };                    

                    Uow.Projects.Add(newProject);

                    Uow.Commit();

                    var projectUser = new ProjectUser()
                    {
                        ProjectID = newProject.ID,
                        UserID = project.UserID
                    };

                    Uow.Commit();
                }
            }
            return Ok<Task>(null);
        }

        [ValidateModel, HttpPost, Route("updateproject")]
        public IHttpActionResult UpdateProject(Project project)
        {
            if (!string.IsNullOrWhiteSpace(project.UserID) && project.ID > 0 && !string.IsNullOrWhiteSpace(project.Name))
            {
                // Check if the project exists
                var projectDetail = Uow.Projects.Set.Where(p => p.ID == project.ID && p.UserID == project.UserID).FirstOrDefault();

                if (projectDetail != null)
                {
                    projectDetail.Name = project.Name;
                    projectDetail.EditDate = DateTime.Now;
                    projectDetail.EditedBy = project.UserID;

                    try
                    {
                        Uow.Commit();
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message;
                        // show error message
                    }
                }
                else
                {
                    // show error message
                }
            }
            return Ok<string>("Project modified successfully.");
        }

        [ValidateModel, HttpPost, Route("deleteproject")]
        public IHttpActionResult DeleteProject(ProjectToDelete project)
        {
            if (!string.IsNullOrWhiteSpace(project.UserID) && project.ProjectID > 0)
            {
                // Check if the project exists
                var projectToDelete = Uow.Projects.Set.Where(p => p.ID == project.ProjectID && p.UserID == project.UserID).FirstOrDefault();

                if (projectToDelete != null)
                {
                    try
                    {
                        Uow.Projects.Delete(projectToDelete);
                        Uow.Commit();
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message;
                        // show error message
                    }
                }
                else
                {
                    // show error message
                }
            }
            return Ok<string>("Task deleted successfully.");
        }
    }
}