using Catalog.API.DTOs;
using Catalog.API.Models;
using System.Linq.Expressions;

namespace Catalog.API.IRepository
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductGetDTO>> GetAllProduct();
        Task<ProductGetDTO> GetProduct(int id);
        Task<int> CreateProduct(ProductPostDTO productPostDTO);
        Task<bool> UpdateProduct(ProductPutDTO productPutDTO);
        Task<bool> DeleteProduct(int id);
        Task<ProductGetDTO> GetProduct(Expression<Func<Product, bool>> expression = null);
    }
}
