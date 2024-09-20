using BuildingBlock.Exceptions;

namespace Catalog.API.Exceptions
{
    public class ProductNotFoundException : NotFoundException
    {


        public ProductNotFoundException(int id) : base("Product", id)
        {
        }
    }

    public class ProductBadRequestException : BadRequestException
    {
        public ProductBadRequestException(string message) : base("Product input is invalid", message)
        {
        }
    }

    public class ProductInternalException : InternalServerException
    {
        public ProductInternalException(string message) : base("Product service has exception ", message)
        {

        }
    }
}
