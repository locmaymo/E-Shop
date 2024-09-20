using AutoMapper;
using Catalog.API.DAO;
using Catalog.API.DTOs;
using Catalog.API.Exceptions;
using Catalog.API.IRepository;
using Catalog.API.Models;

namespace Catalog.API.Repository
{
    public class ProductRepository : EntityDAO<Product>, IProductRepository
    {

        private readonly IMapper _mapper;
        private readonly ILogger<ProductRepository> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string requestId;

        public ProductRepository(CatalogDbContext context, ILogger<ProductRepository> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, logger, httpContextAccessor)
        {
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            requestId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? "No Trace Identifier";
        }

        public async Task<int> CreateProduct(ProductPostDTO productPostDTO)
        {
            _logger.LogInformation("TraceId:{id}, Create {Request} entity ", requestId, typeof(Product).Name);
            try
            {
                _logger.LogInformation("TraceId:{id}, Mapping {dto} to {model} ", requestId, typeof(ProductPostDTO).Name, typeof(Product).Name);
                var Product = _mapper.Map<Product>(productPostDTO);
                var status = await AddEntity(Product);
                return status > 0 ? Product.Id : 0;

            }
            catch (AutoMapperMappingException ex)
            {

                _logger.LogError(ex, "TraceId:{id}. AutoMapper failed to map the {DTO} to {Model}.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", _httpContextAccessor.HttpContext?.TraceIdentifier, typeof(ProductPostDTO).Name, typeof(Product).Name, ex.Message, ex.StackTrace);
                throw;
            }
            catch (ProductInternalException)
            {
                throw;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "TraceId:{id}. An unexpected error occurred while createing the {Request} entity.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", _httpContextAccessor.HttpContext?.TraceIdentifier, typeof(Product).Name, ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task<bool> DeleteProduct(int id)
        {
            _logger.LogInformation("TraceId:{id}, Delete {Request} entity with Id = {Id}", requestId, typeof(Product).Name, id);
            try
            {
                var Product = await GetEntity(c => c.Id == id);
                if (Product is null)
                {

                    throw new ProductNotFoundException(id);
                }
                var status = await DeleteEntity(Product);
                return status > 0;

            }
            catch (ProductNotFoundException ex)
            {

                _logger.LogInformation("TraceId:{id}. Retrieved {Request} entity with Id = {Id} return null.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", requestId, typeof(Product).Name, id, ex.Message, ex.StackTrace);
                throw;
            }
            catch (ProductInternalException)
            {
                throw;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "TraceId:{id}. An unexpected error occurred while deleting the {Request} entity with Id = {Id}.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", _httpContextAccessor.HttpContext?.TraceIdentifier, typeof(Product).Name, id, ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task<IEnumerable<ProductGetDTO>> GetAllProduct()
        {
            _logger.LogInformation("TraceId:{id}, Get {Request} entities}", requestId, typeof(Product).Name);
            try
            {
                var categories = await GetEntities(null, p => p.Category);
                return _mapper.Map<IEnumerable<ProductGetDTO>>(categories);
            }
            catch (AutoMapperMappingException ex)
            {

                _logger.LogError(ex, "TraceId:{id}. AutoMapper failed to map the {Model} to {DTO}.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", _httpContextAccessor.HttpContext?.TraceIdentifier, typeof(Product).Name, typeof(ProductGetDTO).Name, ex.Message, ex.StackTrace);
                throw;
            }
            catch (ProductInternalException)
            {
                throw;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "TraceId:{id}. An unexpected error occurred while retreiving the {Request} entities.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", _httpContextAccessor.HttpContext?.TraceIdentifier, typeof(Product).Name, ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task<ProductGetDTO> GetProduct(int id)
        {
            _logger.LogInformation("TraceId:{id}, Get {Request} entity with Id = {Id}", requestId, typeof(Product).Name, id);
            try
            {
                var Product = await GetEntity(c => c.Id == id, p => p.Category);
                return _mapper.Map<ProductGetDTO>(Product);
            }
            catch (ProductNotFoundException ex)
            {

                _logger.LogInformation("TraceId:{id}. Retrieved {Request} entity with Id = {Id} return null.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", requestId, typeof(Product).Name, id, ex.Message, ex.StackTrace);
                throw;
            }
            catch (AutoMapperMappingException ex)
            {

                _logger.LogError(ex, "TraceId:{id}. AutoMapper failed to map the {Model} to {DTO}.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", _httpContextAccessor.HttpContext?.TraceIdentifier, typeof(Product).Name, typeof(ProductGetDTO).Name, ex.Message, ex.StackTrace);
                throw;
            }
            catch (ProductInternalException)
            {
                throw;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "TraceId:{id}. An unexpected error occurred while retrieving the {Request} entity with Id = {Id}.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", _httpContextAccessor.HttpContext?.TraceIdentifier, typeof(Product).Name, id, ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task<bool> UpdateProduct(ProductPutDTO productPutDTO)
        {
            _logger.LogInformation("TraceId:{id}, Update {Request} entity with Id = {Id}", requestId, typeof(Product).Name, productPutDTO.Id);
            try
            {
                var Product = await GetEntity(c => c.Id == productPutDTO.Id);
                if (Product is null)
                {

                    throw new ProductNotFoundException(productPutDTO.Id);
                }

                // Map the ProductDTO to Product entity
                _logger.LogInformation("TraceId:{id}, Mapping {dto} to {model} ", requestId, typeof(ProductPutDTO).Name, typeof(Product).Name);
                Product = _mapper.Map<Product>(productPutDTO);

                // Update the entity and return status
                var status = await UpdateEntity(Product);
                return status > 0;
            }
            catch (ProductNotFoundException ex)
            {

                _logger.LogInformation("TraceId:{id}. Retrieved {Request} entity with Id = {Id} return null.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", requestId, typeof(Product).Name, productPutDTO.Id, ex.Message, ex.StackTrace);
                throw;
            }
            catch (AutoMapperMappingException ex)
            {

                _logger.LogError(ex, "TraceId:{id}. AutoMapper failed to map the {DTO} to {Model}.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", _httpContextAccessor.HttpContext?.TraceIdentifier, typeof(ProductPostDTO).Name, typeof(Product).Name, ex.Message, ex.StackTrace);
                throw;
            }
            catch (ProductInternalException)
            {
                throw;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "TraceId:{id}. An unexpected error occurred while updating the {Request} entity with Id = {Id}.\n. Exception:{exception}.\n StackTrace:{stackTrace}.", _httpContextAccessor.HttpContext?.TraceIdentifier, typeof(Product).Name, productPutDTO.Id, ex.Message, ex.StackTrace);
                throw;
            }
        }
    }
}
