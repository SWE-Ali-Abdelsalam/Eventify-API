using MediatR;

namespace Eventify.Domain.Common
{
    public abstract class DomainEvent : INotification
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
