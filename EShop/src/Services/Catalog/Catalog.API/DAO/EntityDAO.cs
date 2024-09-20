using BuildingBlock.Exceptions;
using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Catalog.API.DAO
{
    public class EntityDAO<T> : AuditedBaseEntity where T : class, IAuditedEntityBase
    {
        private readonly CatalogDbContext _context;
        private readonly ILogger<EntityDAO<T>> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string requestId;
        public EntityDAO(CatalogDbContext context, ILogger<EntityDAO<T>> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            requestId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? "No Trace Identifier";
        }



        public async Task<T?> GetEntity(Expression<Func<T, bool>> expression = null, params Expression<Func<T, object>>[] includeProperties)
        {
            _logger.LogInformation("TraceId:{id}. Execute Query {Request} entity with {query} in database", requestId, typeof(T).Name, expression);

            try
            {
                var entitiy = _context.Set<T>().Where(expression);
                if (includeProperties.Any())
                {
                    foreach (var property in includeProperties)
                    {
                        entitiy = entitiy.Include(property);
                    }
                }

                _logger.LogInformation("[SUCCESS] TraceId:{id}. Fetch {Request} with {Count} entity ", requestId, typeof(T).Name, entitiy.Count());
                return await entitiy.AsNoTracking().SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("TraceId:{id}. Execute Query {Request} in database catch\n. Exception:{Exception}\n. Stacktrace:{Stacktrace}", requestId, typeof(T).Name, ex.Message, ex.StackTrace);
                throw new InternalServerException(ex.Message);
            }
        }

        public async Task<IEnumerable<T>> GetEntities(Expression<Func<T, bool>> expression = null, params Expression<Func<T, object>>[] includeProperties)
        {
            try
            {

                _logger.LogInformation("TraceId:{id}. Execute Query {Request} entities with {query} in database", requestId, typeof(T).Name, expression);
                IQueryable<T> entities = _context.Set<T>();


                if (expression != null)
                {
                    entities = entities.Where(expression);
                }


                if (includeProperties != null && includeProperties.Any())
                {
                    foreach (var includeProperty in includeProperties)
                    {
                        entities = entities.Include(includeProperty);
                    }
                }
                _logger.LogInformation("[SUCCESS] TraceId:{id}. Fetch {Request} with {Count} entity ", requestId, typeof(T).Name, entities.Count());
                return await entities.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("TraceId:{id}. Execute Query {Request} in database catch\n. Exception:{Exception}\n. Stacktrace:{Stacktrace}", requestId, typeof(T).Name, ex.Message, ex.StackTrace);
                throw new InternalServerException(ex.Message);
            }
        }

        public async Task<int> AddEntity(T entity)
        {
            _logger.LogInformation("TraceId:{id}. Execute Add {Request} entity to database", requestId, typeof(T).Name);
            try
            {


                //handle
                entity.CreatedDate = DateTime.Now;
                await _context.Set<T>().AddAsync(entity);
                int result = await _context.SaveChangesAsync();


                _logger.LogInformation("[SUCCESS] TraceId:{id}. Add {Request} entity with Id ={Id} to database success", requestId, typeof(T).Name, entity.Id);
                return result;
            }
            catch (DbUpdateException dbx)
            {
                _logger.LogError(dbx, "[ERROR] TraceId:{id}. Database occurred while add product {productName} with Id = {Id} to database\n. Exception:{exception}.\n StackTrace:{stackTrace}.",
                             requestId, typeof(T).Name, entity.Id, dbx.Message, dbx.StackTrace);
                throw new InternalServerException(dbx.InnerException.ToString() ?? dbx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] TraceId:{id}. Add {Request} entity with Id = {Id} to database catch\n. Exception:{Exception}\n. Stacktrace:{Stacktrace}.", requestId, typeof(T).Name, entity.Id, ex.Message, ex.StackTrace);
                throw new InternalServerException(ex.Message);
            }
        }

        public async Task<int> UpdateEntity(T entity)
        {
            _logger.LogInformation("TraceId:{id}. Execute Update {Request} entity with Id ={Id} to database", requestId, typeof(T).Name, entity.Id);
            try
            {

                //handle
                entity.LastModifiedDate = DateTime.Now;
                _context.Set<T>().Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
                int result = await _context.SaveChangesAsync();

                _logger.LogInformation("[SUCCESS] TraceId:{id}. Update {Request} entity with Id ={Id} to database success", requestId, typeof(T).Name, entity.Id);

                return result;
            }
            catch (DbUpdateException dbx)
            {
                _logger.LogError(dbx, "[ERROR] TraceId:{id}. Database occurred while update product {productName} with Id = {Id} to database\n. Exception:{exception}.\n StackTrace:{stackTrace}.",
                             requestId, typeof(T).Name, entity.Id, dbx.Message, dbx.StackTrace);
                throw new InternalServerException(dbx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] TraceId:{id}. Update {Request} entity with Id = {Id} to database catch\n. Exception:{exception}\n. Stacktrace:{Stacktrace}.", requestId, typeof(T).Name, entity.Id, ex.Message, ex.StackTrace);
                throw new InternalServerException(ex.Message);
            }
        }


        public async Task<int> DeleteEntity(T entity)
        {
            _logger.LogInformation("TraceId:{id}. Execute Delete {Request} entity with Id ={Id} to database", requestId, typeof(T).Name, entity.Id);
            try
            {

                //handle
                _context.Set<T>().Remove(entity);
                int result = await _context.SaveChangesAsync();

                _logger.LogInformation("[SUCCESS] TraceId:{id}. Delete {Request} entity with Id ={Id} to database success", requestId, typeof(T).Name, entity.Id);
                return result;

            }
            catch (DbUpdateException dbx)
            {
                _logger.LogError(dbx, "[ERROR] TraceId:{id}. Database occurred while delete product {productName} with Id = {Id} to database\n. -Exception:{exception}.\n -StackTrace:{stackTrace}.",
                             requestId, typeof(T).Name, entity.Id, dbx.Message, dbx.StackTrace);
                throw new InternalServerException(dbx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] TraceId:{id}. Delete {Request} entity with Id = {Id} to database catch\n. -Exception:{exception}\n. -Stacktrace:{stackTrace}.", requestId, typeof(T).Name, entity.Id, ex.Message, ex.StackTrace);
                throw new InternalServerException(ex.Message);
            }

        }
    }
}
