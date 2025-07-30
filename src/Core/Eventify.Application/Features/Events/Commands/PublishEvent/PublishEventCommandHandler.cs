using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Events;
using Eventify.Shared.Enums;
using MediatR;

namespace Eventify.Application.Features.Events.Commands.PublishEvent
{
    public class PublishEventCommandHandler : IRequestHandler<PublishEventCommand, Result>
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly ICurrentUserService _currentUserService;

        public PublishEventCommandHandler(IRepository<Event> eventRepository, ICurrentUserService currentUserService)
        {
            _eventRepository = eventRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(PublishEventCommand request, CancellationToken cancellationToken)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
            if (eventEntity == null)
            {
                return Result.Failure("Event not found");
            }

            // Check permission
            if (eventEntity.OrganizerId.ToString() != _currentUserService.UserId &&
                !_currentUserService.IsInRole("Admin"))
            {
                return Result.Failure("You don't have permission to publish this event");
            }

            // Validate event can be published
            if (eventEntity.Status != EventStatus.Draft)
            {
                return Result.Failure("Only draft events can be published");
            }

            // Validate event has minimum required information
            if (string.IsNullOrEmpty(eventEntity.Title) || string.IsNullOrEmpty(eventEntity.Description))
            {
                return Result.Failure("Event must have title and description to be published");
            }

            if (eventEntity.EventDates.StartDate <= DateTime.UtcNow)
            {
                return Result.Failure("Cannot publish events with past start dates");
            }

            eventEntity.Publish();
            await _eventRepository.UpdateAsync(eventEntity, cancellationToken);

            return Result.Success();
        }
    }
}
