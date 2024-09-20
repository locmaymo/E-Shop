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

        public CategoryRepository(CatalogDbContext context, ILogger<CategoryRepository> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        : base(context, logger, httpContextAccessor) // Pass both context and logger to the base class
        {
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> CreateCategory(CategoryPostDTO categoryPostDTO)
        {
            try
            {
                var category = _mapper.Map<Category>(categoryPostDTO);
                var status = await AddEntity(category);
                return status > 0 ? category.Id : 0;

            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<bool> DeleteCategory(int id)
        {
            try
            {
                var category = await GetEntity(c => c.Id == id);
                var status = await DeleteEntity(category);
                return status > 0;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategory()
        {
            var categories = await GetEntities();
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO> GetCategory(int id)
        {
            var category = await GetEntity(c => c.Id == id);
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<bool> UpdateCategory(CategoryDTO categoryDTO)
        {
            try
            {
                string requestId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? "No Trace Identifier";
                _logger.LogInformation("Id: {id}. Update {Request} entity with Id = {Id}", requestId, typeof(Category).Name, categoryDTO.Id);
                var category = await GetEntity(c => c.Id == categoryDTO.Id);
                if (category == null)
                {
                    throw new CategoryNotFoundException(categoryDTO.Id);
                }
                category = _mapper.Map<Category>(categoryDTO);
                var status = await UpdateEntity(category);
                return status > 0;

            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR]Id: {id}. Update {Request} with Id = {Id} catch\n. Exception: {Exception}\n.  Stacktrace: {Stacktrace}\n", _httpContextAccessor.HttpContext.TraceIdentifier, typeof(Category).Name, categoryDTO.Id, ex.Message, ex.StackTrace);
                throw;
            }
        }
    }
}
