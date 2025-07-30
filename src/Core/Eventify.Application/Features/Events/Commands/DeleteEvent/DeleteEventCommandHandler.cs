using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Events;
using Eventify.Shared.Enums;
using MediatR;

namespace Eventify.Application.Features.Events.Commands.DeleteEvent
{
    public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, Result>
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteEventCommandHandler(IRepository<Event> eventRepository, ICurrentUserService currentUserService)
        {
            _eventRepository = eventRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
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
                return Result.Failure("You don't have permission to delete this event");
            }

            // Check if event can be deleted
            if (eventEntity.Status == EventStatus.Published && eventEntity.CurrentRegistrations > 0)
            {
                return Result.Failure("Cannot delete published events with registrations. Cancel the event instead.");
            }

            // Soft delete the event
            await _eventRepository.DeleteAsync(eventEntity, cancellationToken);

            return Result.Success();
        }
    }
}
