using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Events;
using MediatR;

namespace Eventify.Application.Features.Events.Commands.CreateEvent
{
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Result<Guid>>
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<EventCategory> _categoryRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateEventCommandHandler(
            IRepository<Event> eventRepository,
            IRepository<EventCategory> categoryRepository,
            ICurrentUserService currentUserService)
        {
            _eventRepository = eventRepository;
            _categoryRepository = categoryRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            // Verify the user has permission to create events
            if (!_currentUserService.IsInRole("Organizer") && !_currentUserService.IsInRole("Admin"))
            {
                return Result<Guid>.Failure("You don't have permission to create events");
            }

            // Verify category exists
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category == null)
            {
                return Result<Guid>.Failure("Invalid category selected");
            }

            // Create the event
            var eventEntity = new Event(
                request.Title,
                request.Description,
                request.StartDate,
                request.EndDate,
                request.OrganizerId,
                request.CategoryId,
                request.VenueId,
                request.MaxCapacity,
                request.IsVirtual
            );

            eventEntity.UpdateDetails(
                request.Title,
                request.Description,
                request.ShortDescription,
                request.StartDate,
                request.EndDate,
                request.MaxCapacity,
                request.IsVirtual
            );

            eventEntity.SetVisibility(request.IsPublic);
            eventEntity.SetApprovalRequirement(request.RequiresApproval);

            eventEntity.UpdateVenue(request.VenueId);

            var createdEvent = await _eventRepository.AddAsync(eventEntity, cancellationToken);

            return Result<Guid>.Success(createdEvent.Id);
        }
    }
}
