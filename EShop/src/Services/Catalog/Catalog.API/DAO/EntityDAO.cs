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
        public EntityDAO(CatalogDbContext context, ILogger<EntityDAO<T>> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }



        public async Task<T?> GetEntity(Expression<Func<T, bool>> expression = null, params Expression<Func<T, object>>[] includeProperties)
        {



            try
            {
                _logger.LogInformation("Id: {id}. Get {Request} with {Query}", _httpContextAccessor.HttpContext.TraceIdentifier, typeof(T).Name, expression);
                var entitiy = _context.Set<T>().Where(expression);
                if (includeProperties.Any())
                {
                    foreach (var property in includeProperties)
                    {
                        entitiy = entitiy.Include(property);
                    }
                }

                _logger.LogInformation("[SUCCESS] Id: {id}. Fetch {Request} with {Count} entity ", _httpContextAccessor.HttpContext.TraceIdentifier, typeof(T).Name, entitiy.Count());
                return await entitiy.AsNoTracking().SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] Id: {id}. Get {Request} catch\n. Exception: {Exception}\n. Stacktrace: {Stacktrace}", _httpContextAccessor.HttpContext.TraceIdentifier, typeof(T).Name, ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task<IEnumerable<T>> GetEntities(Expression<Func<T, bool>> expression = null, params Expression<Func<T, object>>[] includeProperties)
        {
            try
            {

                _logger.LogInformation(" Id: {id}. Get {Request}  with {Query}", _httpContextAccessor.HttpContext.TraceIdentifier, typeof(T).Name, expression);
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
                _logger.LogInformation("[SUCCESS] Id: {id}. Fetch {Request}  entity ", _httpContextAccessor.HttpContext.TraceIdentifier, typeof(T).Name);
                return await entities.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] Id: {id}. Get {Request} catch\n. Exception: {Exception}\n. Stacktrace: {Stacktrace}", _httpContextAccessor.HttpContext.TraceIdentifier, typeof(T).Name, ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task<int> AddEntity(T entity)
        {
            try
            {
                _logger.LogInformation("Id: {id}. Add {Request} entity", _httpContextAccessor.HttpContext.TraceIdentifier, typeof(T).Name);

                //handle
                await _context.Set<T>().AddAsync(entity);
                int result = await _context.SaveChangesAsync();


                _logger.LogInformation("[SUCCESS] Id: {id}. Add {Request}  entity", _httpContextAccessor.HttpContext.TraceIdentifier, typeof(T).Name);
                return result;
            }
            catch (DbUpdateException dbx)
            {
                _logger.LogError(dbx, "[ERROR] Id: {id}. Database occurred while adding product {productName}\n. Error: {error}\n. StackTrace: {stackTrace}\n. ",
                             _httpContextAccessor.HttpContext.TraceIdentifier, typeof(T).Name, dbx.Message, dbx.StackTrace);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] Id: {id}. Add {Request} catch\n. Exception: {Exception}\n. Stacktrace: {Stacktrace} ", _httpContextAccessor.HttpContext.TraceIdentifier, typeof(T).Name, ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task<int> UpdateEntity(T entity)
        {
            try
            {




                //handle
                _context.Set<T>().Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
                int result = await _context.SaveChangesAsync();

                _logger.LogInformation("[SUCCESS] Id: {id}. Update {Request} entity with Id = {Id}", _httpContextAccessor.HttpContext.TraceIdentifier, typeof(T).Name, entity.Id);

                return result;
            }
            catch (DbUpdateException dbx)
            {
                _logger.LogError(dbx, "[ERROR] Id: {id}. Database occurred while update product {productName} with Id = {Id}\n. Error: {error}.\n StackTrace: {stackTrace}\n.  ",
                             _httpContextAccessor.HttpContext.TraceIdentifier, typeof(T).Name, entity.Id, dbx.Message, dbx.StackTrace);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] Id: {id}. Update {Request} with Id = {Id}\n catch. Exception: {Exception}\n. Stacktrace: {Stacktrace} ", _httpContextAccessor.HttpContext.TraceIdentifier, typeof(T).Name, entity.Id, ex.Message, ex.StackTrace);
                throw;
            }
        }


        public async Task<int> DeleteEntity(T entity)
        {
            try
            {
                _logger.LogInformation(" Id: {id}. Delete {Request} entity with Id = {Id}", _httpContextAccessor.HttpContext.TraceIdentifier, typeof(T).Name, entity.Id);

                //handle
                _context.Set<T>().Remove(entity);
                int result = await _context.SaveChangesAsync();

                _logger.LogInformation("[SUCCESS] Id: {id}. Delete {Request} entity with Id = {Id}", _httpContextAccessor.HttpContext.TraceIdentifier, typeof(T).Name, entity.Id);
                return result;

            }
            catch (DbUpdateException dbx)
            {
                _logger.LogError(dbx, "[ERROR] Id: {id}. Database occurred while delete product {productName} with Id = {Id}\n. Error: {error}.\n StackTrace: {stackTrace}\n.  ",
                            _httpContextAccessor.HttpContext.TraceIdentifier, typeof(T).Name, entity.Id, dbx.Message, dbx.StackTrace);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] Id: {id}. Delete {Request} with Id = {Id} catch\n. Exception: {Exception}\n. Stacktrace: {Stacktrace}", _httpContextAccessor.HttpContext.TraceIdentifier, typeof(T).Name, entity.Id, ex.Message, ex.StackTrace);
                throw;
            }

        }
    }
}
