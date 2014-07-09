using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TaskList.Api.Filters;
using TaskList.Api.Models;
using TaskList.Model.Models;

namespace TaskList.Api.Controllers
{
    [RoutePrefix("api/Task")]
    public class TaskController : BaseController<Task>
    {
        public TaskController(): base() {}

        [Queryable, HttpGet, Route("GetTasks")]
        public IHttpActionResult GetTasksByUser(string userID, int? projectID)
        {
            List<Task> tasks = null;
            string projectName = null;
            bool isDefaultProject = false;
            if (projectID == null || projectID == 0)
            {
                isDefaultProject = true;
                tasks = Uow.Tasks.Set.Where(t => (
                    t.User.Id == userID
                )).OrderBy(t => t.Order).ToList();
            }
            else
            {
                tasks = Uow.Tasks.Set.Where(t => (
                    t.User.Id == userID && t.ProjectID == projectID
                )).OrderBy(t => t.Order).ToList();

                var project = Uow.Projects.Set.Where(p => p.ID == projectID).FirstOrDefault();
                
                projectName = (project != null) ? project.Name : string.Empty;
                isDefaultProject = (project != null) ? project.IsDefault : false;
            }

            return Ok(new { ProjectName = projectName, IsDefaultProject = isDefaultProject, Tasks = tasks.Select(t => new { ID = t.ID, Name = t.Name, Order = t.Order, ProjectID = t.ProjectID, IsSelected = false }) });
        }

        [HttpPost]
        public override IHttpActionResult Post(Task task)
        {
            if (!string.IsNullOrWhiteSpace(task.UserID) && task.ProjectID > 0 && !string.IsNullOrWhiteSpace(task.Name))
            {
                // Check if the userID and project exists and if they are related
                var project = Uow.Projects.Set.Where(p => p.ID == task.ProjectID && p.User.Id == task.UserID).FirstOrDefault();

                if (project != null)
                {
                    int taskOrder = Uow.Tasks.Set.Where(t => t.UserID == task.UserID && t.ProjectID == task.ProjectID).ToList().Count;
                    
                    // increment the order of the new task
                    var newTask = new Task(){
                        ID = 0,
                        Name = task.Name,
                        CreatedBy = task.UserID,
                        CreationDate = DateTime.Now,
                        ProjectID = task.ProjectID,
                        UserID = task.UserID,
                        CategoryID = task.CategoryID,
                    };

                    Uow.Tasks.Add(newTask);
                    Uow.Commit();
                }
            }
            return Ok<Task>(null);
        }

        [ValidateModel, HttpPost, Route("updatetask")]
        public IHttpActionResult UpdateTask(Task task)
        {
            if (!string.IsNullOrWhiteSpace(task.UserID) && task.ProjectID > 0 && task.ID > 0 && !string.IsNullOrWhiteSpace(task.Name))
            {
                // Check if the task exists
                var taskDetail = Uow.Tasks.Set.Where(t => t.ID == task.ID && t.UserID == task.UserID && t.ProjectID == task.ProjectID).FirstOrDefault();

                if (taskDetail != null)
                {
                    taskDetail.Name = task.Name;
                    taskDetail.EditDate = DateTime.Now;
                    taskDetail.EditedBy = task.UserID;

                    try
                    {
                        Uow.Commit();
                    }catch(Exception ex)
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
            return Ok<string>("Task modified successfully.");
        }

        [ValidateModel, HttpPost, Route("deletetask")]
        public IHttpActionResult DeleteTask(TaskToDelete task)
        {
            if (!string.IsNullOrWhiteSpace(task.UserID) && task.ProjectID > 0 && task.TaskID > 0)
            {
                // Check if the task exists
                var taskToDelete = Uow.Tasks.Set.Where(t => t.ID == task.TaskID && t.UserID == task.UserID && t.ProjectID == task.ProjectID).FirstOrDefault();

                if (taskToDelete != null)
                {
                    try
                    {
                        Uow.Tasks.Delete(taskToDelete);
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