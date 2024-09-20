using Catalog.API.DTOs;

namespace Catalog.API.IRepository
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategory();
        Task<CategoryDTO> GetCategory(int id);
        Task<int> CreateCategory(CategoryPostDTO categoryPostDTO);
        Task<bool> UpdateCategory(CategoryPutDTO categoryPutDTO);
        Task<bool> DeleteCategory(int id);
    }
}
