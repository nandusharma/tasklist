using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TaskList.Model.Models;

namespace TaskList.Api.Controllers
{
    [RoutePrefix("api/Category")]
    public class CategoryController : BaseController<Category>
    {
        public CategoryController() : base() {}

        [Queryable, HttpGet, Route("All")]
        public IHttpActionResult GetCategories()
        {
            var categories = Uow.Categories.Set.ToList();

            return Ok(categories.Select(c => new { ID = c.ID, Name = c.Name}));
        }
    }
}