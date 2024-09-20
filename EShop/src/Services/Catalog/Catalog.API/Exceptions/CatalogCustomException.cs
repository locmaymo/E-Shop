using BuildingBlock.Exceptions;

namespace Catalog.API.Exceptions
{
    public class CategoryNotFoundException : NotFoundException
    {


        public CategoryNotFoundException(int id) : base("Category", id)
        {
        }
    }

    public class ProductNotFoundException : NotFoundException
    {


        public ProductNotFoundException(int id) : base("Product", id)
        {
        }
    }
}
