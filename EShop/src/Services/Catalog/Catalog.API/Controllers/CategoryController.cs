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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        private readonly ILogger<CategoryController> _logger;


        public CategoryController(ICategoryRepository categoryRepository, ILogger<CategoryController> logger)
        {
            _categoryRepository = categoryRepository;

            _logger = logger;


        }
        private string RequestId => HttpContext?.TraceIdentifier ?? "No Trace Identifier";

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            _logger.LogInformation("[Start] TraceId:{Id}, Received request:{Path}", RequestId, HttpContext.Request.Path);

            try
            {
                var categories = await _categoryRepository.GetAllCategory();

                return Ok(categories);
            }
            catch (Exception ex) when (ex is AutoMapperMappingException or CategoryInternalException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CategoryInternalException(ex.Message);
            }


        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {

            _logger.LogInformation("[Start] TraceId:{Id}, Received request:{Path}", RequestId, HttpContext.Request.Path);
            try
            {
                var category = await _categoryRepository.GetCategory(id);
                return Ok(category);
            }
            catch (Exception ex) when (ex is AutoMapperMappingException or CategoryInternalException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CategoryInternalException(ex.Message);
            }



        }

        [HttpPost]
        public async Task<IActionResult> PostCategory(CategoryPostDTO categoryPostDTO)
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
                var categoryId = await _categoryRepository.CreateCategory(categoryPostDTO);
                return Ok(categoryId);
            }
            catch (Exception ex) when (ex is AutoMapperMappingException or CategoryInternalException or CategoryBadRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CategoryInternalException(ex.Message);
            }

        }

        [HttpPut]
        public async Task<IActionResult> PutCategory(CategoryPutDTO categoryDTO)
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
                var response = await _categoryRepository.UpdateCategory(categoryDTO);
                return Ok();
            }
            catch (Exception ex) when (ex is CategoryNotFoundException or AutoMapperMappingException or CategoryInternalException or BadRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CategoryInternalException(ex.Message);
            }


        }


        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            _logger.LogInformation("[Start] TraceId:{Id}, Received request:{Path}", RequestId, HttpContext.Request.Path);
            try
            {
                var response = await _categoryRepository.DeleteCategory(id);
                return Ok();
            }
            catch (Exception ex) when (ex is CategoryNotFoundException or CategoryInternalException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CategoryInternalException(ex.Message);
            }

        }
    }
}
