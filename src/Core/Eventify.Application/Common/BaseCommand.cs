using MediatR;

namespace Eventify.Application.Common
{
    public abstract class BaseCommand : IRequest
    {
        public Guid RequestId { get; } = Guid.NewGuid();
        public DateTime RequestedAt { get; } = DateTime.UtcNow;
    }

    public abstract class BaseCommand<TResponse> : IRequest<TResponse>
    {
        public Guid RequestId { get; } = Guid.NewGuid();
        public DateTime RequestedAt { get; } = DateTime.UtcNow;
    }
}
