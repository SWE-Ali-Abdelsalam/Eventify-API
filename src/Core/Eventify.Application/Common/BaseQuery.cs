using MediatR;

namespace Eventify.Application.Common
{
    public abstract class BaseQuery<TResponse> : IRequest<TResponse>
    {
        public Guid RequestId { get; } = Guid.NewGuid();
        public DateTime RequestedAt { get; } = DateTime.UtcNow;
    }
}
