using MediatR;

namespace BuildingBlock.CQRS
{
    public interface IQuery : IQuery<Unit>
    {

    }

    public interface IQuery<out TResponse> : IRequest<TResponse>
    {
    }
}
