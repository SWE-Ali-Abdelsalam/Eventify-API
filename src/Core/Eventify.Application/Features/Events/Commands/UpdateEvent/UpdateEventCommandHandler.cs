using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Events;
using MediatR;

namespace Eventify.Application.Features.Events.Commands.UpdateEvent
{
    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, Result>
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateEventCommandHandler(IRepository<Event> eventRepository, ICurrentUserService currentUserService)
        {
            _eventRepository = eventRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
            if (eventEntity == null)
            {
                return Result.Failure("Event not found");
            }

            // Check permission - only the organizer or admin can update
            if (eventEntity.OrganizerId.ToString() != _currentUserService.UserId &&
                !_currentUserService.IsInRole("Admin"))
            {
                return Result.Failure("You don't have permission to update this event");
            }

            // Update event details
            eventEntity.UpdateDetails(
                request.Title,
                request.Description,
                request.ShortDescription,
                request.StartDate,
                request.EndDate,
                request.MaxCapacity,
                request.IsVirtual
            );

            eventEntity.UpdateVenue(request.VenueId);
            eventEntity.SetVisibility(request.IsPublic);
            eventEntity.SetApprovalRequirement(request.RequiresApproval);

            if (request.RegistrationOpenDate.HasValue && request.RegistrationCloseDate.HasValue)
            {
                eventEntity.SetRegistrationPeriod(request.RegistrationOpenDate.Value, request.RegistrationCloseDate.Value);
            }

            await _eventRepository.UpdateAsync(eventEntity, cancellationToken);

            return Result.Success();
        }
    }
}
