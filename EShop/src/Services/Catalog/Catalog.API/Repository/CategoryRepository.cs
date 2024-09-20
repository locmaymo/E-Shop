using AutoMapper;
using Catalog.API.DAO;
using Catalog.API.DTOs;
using Catalog.API.Exceptions;
using Catalog.API.IRepository;
using Catalog.API.Models;

namespace Catalog.API.Repository
{
    public class CategoryRepository : EntityDAO<Category>, ICategoryRepository
    {

        private readonly IMapper _mapper;
        private readonly ILogger<CategoryRepository> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string requestId;

        public CategoryRepository(CatalogDbContext context, ILogger<CategoryRepository> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        : base(context, logger, httpContextAccessor) // Pass both context and logger to the base class
        {
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            requestId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? "No Trace Identifier";
        }

        public async Task<int> CreateCategory(CategoryPostDTO categoryPostDTO)
        {
            _logger.LogInformation("TraceId:{id}, Create {Request} entity ", requestId, typeof(Category).Name);
            try
            {
                _logger.LogInformation("TraceId:{id}, Mapping {dto} to {model} ", requestId, typeof(CategoryPostDTO).Name, typeof(Category).Name);
                var category = _mapper.Map<Category>(categoryPostDTO);
                var status = await AddEntity(category);
                return status > 0 ? category.Id : 0;

            }
            catch (AutoMapperMappingException ex)
            {

                _logger.LogError(ex, "TraceId:{id}. AutoMapper failed to map the {DTO} to {Model}.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", _httpContextAccessor.HttpContext?.TraceIdentifier, typeof(CategoryPostDTO).Name, typeof(Category).Name, ex.Message, ex.StackTrace);
                throw;
            }
            catch (CategoryInternalException)
            {
                throw;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "TraceId:{id}. An unexpected error occurred while createing the {Request} entity.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", _httpContextAccessor.HttpContext?.TraceIdentifier, typeof(Category).Name, ex.Message, ex.StackTrace);
                throw;
            }

        }

        public async Task<bool> DeleteCategory(int id)
        {
            _logger.LogInformation("TraceId:{id}, Delete {Request} entity with Id = {Id}", requestId, typeof(Category).Name, id);
            try
            {
                var category = await GetEntity(c => c.Id == id);
                if (category is null)
                {

                    throw new CategoryNotFoundException(id);
                }
                var status = await DeleteEntity(category);
                return status > 0;

            }
            catch (CategoryNotFoundException ex)
            {

                _logger.LogInformation("TraceId:{id}. Retrieved {Request} entity with Id = {Id} return null.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", requestId, typeof(Category).Name, id, ex.Message, ex.StackTrace);
                throw;
            }
            catch (CategoryInternalException)
            {
                throw;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "TraceId:{id}. An unexpected error occurred while deleting the {Request} entity with Id = {Id}.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", _httpContextAccessor.HttpContext?.TraceIdentifier, typeof(Category).Name, id, ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategory()
        {
            _logger.LogInformation("TraceId:{id}, Get {Request} entities}", requestId, typeof(Category).Name);
            try
            {
                var categories = await GetEntities();
                return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
            }
            catch (AutoMapperMappingException ex)
            {

                _logger.LogError(ex, "TraceId:{id}. AutoMapper failed to map the {Model} to {DTO}.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", _httpContextAccessor.HttpContext?.TraceIdentifier, typeof(Category).Name, typeof(CategoryDTO).Name, ex.Message, ex.StackTrace);
                throw;
            }
            catch (CategoryInternalException)
            {
                throw;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "TraceId:{id}. An unexpected error occurred while retreiving the {Request} entities.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", _httpContextAccessor.HttpContext?.TraceIdentifier, typeof(Category).Name, ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task<CategoryDTO> GetCategory(int id)
        {

            _logger.LogInformation("TraceId:{id}, Get {Request} entity with Id = {Id}", requestId, typeof(Category).Name, id);
            try
            {
                var category = await GetEntity(c => c.Id == id);
                return _mapper.Map<CategoryDTO>(category);
            }
            catch (CategoryNotFoundException ex)
            {

                _logger.LogInformation("TraceId:{id}. Retrieved {Request} entity with Id = {Id} return null.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", requestId, typeof(Category).Name, id, ex.Message, ex.StackTrace);
                throw;
            }
            catch (AutoMapperMappingException ex)
            {

                _logger.LogError(ex, "TraceId:{id}. AutoMapper failed to map the {Model} to {DTO}.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", _httpContextAccessor.HttpContext?.TraceIdentifier, typeof(Category).Name, typeof(CategoryDTO).Name, ex.Message, ex.StackTrace);
                throw;
            }
            catch (CategoryInternalException)
            {
                throw;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "TraceId:{id}. An unexpected error occurred while retrieving the {Request} entity with Id = {Id}.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", _httpContextAccessor.HttpContext?.TraceIdentifier, typeof(Category).Name, id, ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task<bool> UpdateCategory(CategoryPutDTO categoryDTO)
        {

            _logger.LogInformation("TraceId:{id}, Update {Request} entity with Id = {Id}", requestId, typeof(Category).Name, categoryDTO.Id);
            try
            {
                var category = await GetEntity(c => c.Id == categoryDTO.Id);
                if (category is null)
                {

                    throw new CategoryNotFoundException(categoryDTO.Id);
                }

                // Map the categoryDTO to Category entity
                _logger.LogInformation("TraceId:{id}, Mapping {dto} to {model} ", requestId, typeof(CategoryPutDTO).Name, typeof(Category).Name);
                category = _mapper.Map<Category>(categoryDTO);

                // Update the entity and return status
                var status = await UpdateEntity(category);
                return status > 0;
            }
            catch (CategoryNotFoundException ex)
            {

                _logger.LogInformation("TraceId:{id}. Retrieved {Request} entity with Id = {Id} return null.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", requestId, typeof(Category).Name, categoryDTO.Id, ex.Message, ex.StackTrace);
                throw;
            }
            catch (AutoMapperMappingException ex)
            {

                _logger.LogError(ex, "TraceId:{id}. AutoMapper failed to map the {DTO} to {Model}.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", _httpContextAccessor.HttpContext?.TraceIdentifier, typeof(CategoryPostDTO).Name, typeof(Category).Name, ex.Message, ex.StackTrace);
                throw;
            }
            catch (CategoryInternalException)
            {
                throw;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "TraceId:{id}. An unexpected error occurred while updating the {Request} entity with Id = {Id}.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", _httpContextAccessor.HttpContext?.TraceIdentifier, typeof(Category).Name, categoryDTO.Id, ex.Message, ex.StackTrace);
                throw;
            }

        }
    }
}
