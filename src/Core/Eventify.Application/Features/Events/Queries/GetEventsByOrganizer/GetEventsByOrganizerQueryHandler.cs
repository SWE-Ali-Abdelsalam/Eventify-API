using Eventify.Application.Common;
using Eventify.Application.Features.Events.DTOs;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Events;
using Eventify.Domain.Specifications.EventSpecifications;
using MediatR;

namespace Eventify.Application.Features.Events.Queries.GetEventsByOrganizer
{
    public class GetEventsByOrganizerQueryHandler : IRequestHandler<GetEventsByOrganizerQuery, Result<List<EventSummaryDto>>>
    {
        private readonly IRepository<Event> _eventRepository;

        public GetEventsByOrganizerQueryHandler(IRepository<Event> eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<Result<List<EventSummaryDto>>> Handle(GetEventsByOrganizerQuery request, CancellationToken cancellationToken)
        {
            var spec = new EventsByOrganizerSpecification(request.OrganizerId);
            spec.ApplyPaging((request.PageNumber - 1) * request.PageSize, request.PageSize);

            var events = await _eventRepository.ListBySpecAsync(spec, cancellationToken);

            var eventDtos = events.Select(e => new EventSummaryDto
            {
                Id = e.Id,
                Title = e.Title,
                ShortDescription = e.ShortDescription,
                StartDate = e.EventDates.StartDate,
                EndDate = e.EventDates.EndDate,
                Status = e.Status.ToString(),
                IsPublic = e.IsPublic,
                IsVirtual = e.IsVirtual,
                MaxCapacity = e.MaxCapacity,
                CurrentRegistrations = e.CurrentRegistrations,
                AvailableSpots = e.AvailableSpots,
                ImageUrl = e.ImageUrl,
                Category = new EventCategoryDto
                {
                    Id = e.Category.Id,
                    Name = e.Category.Name,
                    Color = e.Category.Color,
                    IconUrl = e.Category.IconUrl
                },
                Organizer = new EventOrganizerDto
                {
                    Id = e.Organizer.Id,
                    Name = e.Organizer.FullName,
                    Email = e.Organizer.Email
                },
                Venue = e.Venue != null ? new VenueSummaryDto
                {
                    Id = e.Venue.Id,
                    Name = e.Venue.Name,
                    City = e.Venue.Address.City,
                    Country = e.Venue.Address.Country
                } : null,
                TicketTypes = e.TicketTypes.Select(tt => new TicketTypeSummaryDto
                {
                    Id = tt.Id,
                    Name = tt.Name,
                    Price = tt.Price.Amount,
                    Currency = tt.Price.Currency,
                    AvailableQuantity = tt.AvailableQuantity,
                    IsAvailable = tt.IsAvailable
                }).ToList()
            }).ToList();

            return Result<List<EventSummaryDto>>.Success(eventDtos);
        }
    }
}
