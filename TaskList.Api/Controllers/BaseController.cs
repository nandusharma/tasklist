using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using TaskList.Api.Filters;
using TaskList.Data;
using TaskList.Data.Interfaces;
using Microsoft.AspNet.Identity;
using TaskList.Model.Interfaces;

namespace TaskList.Api.Controllers
{
    //[Authorize]
    public abstract class BaseController<TEntity> : ApiController where TEntity : class, IHasId
    {
        public IUnitOfWork Uow;
        public IRepository<TEntity> Repo;
        public BaseController()
            : this(UnitOfWork.Instantiate())
        {
        }

        public BaseController(IUnitOfWork uow)
        {
            Uow = uow;
            Repo = Uow.GetRepositoryForType<TEntity>();
        }

        // DELETE api/Base/5
        /// <summary>
        /// Api endpoint for deleting a specific entity by its Id value.
        /// </summary>
        /// <param name="id">Id value of entity to be deleted.</param>
        /// <returns>Deleted entity.</returns>
        public virtual IHttpActionResult Delete(int id)
        {
            TEntity entity = Repo.Find(id);
            if (entity == null)
            {
                return NotFound();
            }

            Repo.Delete(entity);
            Uow.Commit(User.Identity.GetUserId());

            return Ok(entity);
        }

        // GET api/TEntity
        /// <summary>
        /// Api endpoint for entity queries.
        /// </summary>
        /// <returns>Collection of resulting entities.</returns>
        [QueryableAttribute(MaxExpansionDepth = 3)]
        public virtual IQueryable<TEntity> GetAll()
        {
            return Repo.Get();
        }

        // GET api/TEntity/5
        /// <summary>
        /// Api endpoint for getting specific entities by an Id value.
        /// </summary>
        /// <param name="id">Id value of entity required.</param>
        /// <returns>Entity with Id value provided.</returns>
        public virtual IHttpActionResult GetById(int id)
        {
            TEntity entity = Repo.Find(id);
            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        // POST api/Base
        /// <summary>
        /// Api endpoint for creating a new entity.
        /// </summary>
        /// <param name="entity">Serialized representation of created object.</param>
        /// <returns>Created version of entity.</returns>
        [ValidateModel]
        public virtual IHttpActionResult Post(TEntity entity)
        {
            Repo.Add(entity);
            Uow.Commit(User.Identity.GetUserId());

            return CreatedAtRoute("DefaultApi", new { id = entity.ID }, entity);
        }

        // PUT api/TEntity/5
        /// <summary>
        /// Api endpoint for updating an entity.
        /// </summary>
        /// <param name="id">Id value of entity to be updated.</param>
        /// <param name="entity">Serialized representation of updated object.</param>
        /// <returns>Updated version of entity.</returns>
        [ValidateModel]
        public virtual IHttpActionResult Put(int id, TEntity entity)
        {
            if (!id.Equals(entity.ID))
            {
                return BadRequest();
            }

            Repo.Update(entity);

            try
            {
                Uow.Commit(User.Identity.GetUserId());
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Uow.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EntityExists(int id)
        {
            return Repo.Exists(id);
        }
    }
}