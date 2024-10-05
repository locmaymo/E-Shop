using AutoMapper;
using BuildingBlock.Exceptions;
using Catalog.API.DTOs;
using Catalog.API.Exceptions;
using Catalog.API.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        private readonly ILogger<ProductController> _logger;


        public ProductController(IProductRepository productRepository, ILogger<ProductController> logger)
        {
            _productRepository = productRepository;

            _logger = logger;


        }
        private string RequestId => HttpContext?.TraceIdentifier ?? "No Trace Identifier";

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            _logger.LogInformation("[Start] TraceId:{Id}, Received request:{Path}", RequestId, HttpContext.Request.Path);

            try
            {
                var categories = await _productRepository.GetAllProduct();

                return Ok(categories);
            }
            catch (Exception ex) when (ex is AutoMapperMappingException or ProductInternalException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ProductInternalException(ex.Message);
            }


        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {

            _logger.LogInformation("[Start] TraceId:{Id}, Received request:{Path}", RequestId, HttpContext.Request.Path);
            try
            {
                var product = await _productRepository.GetProduct(id);
                return Ok(product);
            }
            catch (Exception ex) when (ex is AutoMapperMappingException or ProductInternalException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ProductInternalException(ex.Message);
            }



        }

        [HttpPost]
        public async Task<IActionResult> PostProduct(ProductPostDTO productPostDTO)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("TraceId:{Id}, Invalid Data in the Request Body", RequestId);
                string errorDetails = string.Join("; ", ModelState.Values
                                                    .SelectMany(v => v.Errors)
                                                    .Select(e => e.ErrorMessage));

                throw new ProductBadRequestException(errorDetails);
            }

            _logger.LogInformation("[Start] TraceId:{Id}, Received request:{Path}", RequestId, HttpContext.Request.Path);
            try
            {
                var productId = await _productRepository.CreateProduct(productPostDTO);
                return Ok(productId);
            }
            catch (Exception ex) when (ex is AutoMapperMappingException or ProductInternalException or ProductBadRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ProductInternalException(ex.Message);
            }

        }

        [HttpPut]
        public async Task<IActionResult> PutProduct(ProductPutDTO productPutDTO)
        {

            _logger.LogInformation("[Start] TraceId:{Id}, Received request:{Path}", RequestId, HttpContext.Request.Path);

            if (!ModelState.IsValid)
            {
                _logger.LogError("TraceId:{Id}, Invalid Data in the Request Body", RequestId);
                string errorDetails = string.Join("; ", ModelState.Values
                                                    .SelectMany(v => v.Errors)
                                                    .Select(e => e.ErrorMessage));

                throw new BadRequestException(errorDetails);
            }
            try
            {
                var response = await _productRepository.UpdateProduct(productPutDTO);
                return Ok();
            }
            catch (Exception ex) when (ex is ProductNotFoundException or AutoMapperMappingException or ProductInternalException or BadRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ProductInternalException(ex.Message);
            }


        }


        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            _logger.LogInformation("[Start] TraceId:{Id}, Received request:{Path}", RequestId, HttpContext.Request.Path);
            try
            {
                var response = await _productRepository.DeleteProduct(id);
                return Ok();
            }
            catch (Exception ex) when (ex is ProductNotFoundException or ProductInternalException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ProductInternalException(ex.Message);
            }

        }

        [HttpPost("ValidProducts")]
        public async Task<IActionResult> GetValidProducts(Dictionary<int, int> quantityOfProducts)
        {
            try
            {
                var validProductIds = new List<int>();
                foreach (var product in quantityOfProducts)
                {
                    var validProduct = await _productRepository.GetProduct(p => p.Id == product.Key && p.IsActice == true && p.Quantity > 0 && p.Quantity >= product.Value);
                    if (validProduct != null)
                    {
                        validProductIds.Add(product.Key);
                    }
                }
                if (validProductIds.Any())
                {
                    return Ok(validProductIds);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                throw new ProductInternalException(ex.Message);
            }
        }
    }
}
