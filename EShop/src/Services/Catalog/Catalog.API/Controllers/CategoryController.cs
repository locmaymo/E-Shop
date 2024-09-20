using Catalog.API.DTOs;
using Catalog.API.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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

        [HttpGet]
        public async Task<IActionResult> GetCategory()
        {


            try
            {
                var categories = await _categoryRepository.GetAllCategory();

                return Ok(categories);
            }
            catch (Exception ex)
            {

                return BadRequest();
            }


        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryRepository.GetCategory(id);
            return Ok(category);

        }

        [HttpPost]
        public async Task<IActionResult> PostCategory(CategoryPostDTO categoryPostDTO)
        {
            var categoryId = await _categoryRepository.CreateCategory(categoryPostDTO);
            return Ok(categoryId);
        }

        [HttpPut]
        public async Task<IActionResult> PutCategory(CategoryDTO categoryDTO)
        {
            var serializedRequestData = JsonSerializer.Serialize(new
            {
                Id = categoryDTO.Id,
                Name = categoryDTO.Name,
            });



            _logger.LogInformation("[Start] Id:{Id} Get request: {Path}", HttpContext.TraceIdentifier, HttpContext.Request.Path);
            try
            {




                var response = await _categoryRepository.UpdateCategory(categoryDTO);




                return Ok();
            }
            catch (Exception ex)
            {



                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);


            }
            finally
            {
                _logger.LogInformation("[END] Id:{Id} Close request: {Path}.", HttpContext.TraceIdentifier, HttpContext.Request.Path);

            }
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var response = await _categoryRepository.DeleteCategory(id);
            return Ok();
        }
    }
}
