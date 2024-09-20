using BuildingBlock.Exceptions;

namespace Catalog.API.Exceptions
{
    //
    public class CategoryNotFoundException : NotFoundException
    {


        public CategoryNotFoundException(int id) : base("Category", id)
        {
        }
    }

    public class CategoryBadRequestException : BadRequestException
    {
        public CategoryBadRequestException(string message) : base("Category input is invalid", message)
        {
        }
    }

    public class CategoryInternalException : InternalServerException
    {
        public CategoryInternalException(string message) : base("Category service has exception ", message)
        {

        }
    }



}
