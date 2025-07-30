using Eventify.Domain.Common;

namespace Eventify.Domain.Events.EventEvents
{
    public class UserProfileUpdatedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string FirstName { get; }
        public string LastName { get; }

        public UserProfileUpdatedEvent(Guid userId, string firstName, string lastName)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
