using Eventify.Application.Common;
using Eventify.Application.Features.Events.DTOs;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Events;
using Eventify.Domain.Specifications.EventSpecifications;
using MediatR;

namespace Eventify.Application.Features.Events.Queries.GetEvents
{
    public class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, Result<PaginatedResult<EventSummaryDto>>>
    {
        private readonly IRepository<Event> _eventRepository;

        public GetEventsQueryHandler(IRepository<Event> eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<Result<PaginatedResult<EventSummaryDto>>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
        {
            var spec = new EventsWithFiltersSpecification(
                request.SearchTerm,
                request.CategoryId,
                request.StartDate,
                request.EndDate,
                request.Status,
                request.PublicOnly
            );

            // Apply pagination
            spec.ApplyPaging((request.PageNumber - 1) * request.PageSize, request.PageSize);

            // Apply sorting
            switch (request.SortBy?.ToLower())
            {
                case "title":
                    if (request.SortDescending)
                        spec.ApplyOrderByDescending(e => e.Title);
                    else
                        spec.ApplyOrderBy(e => e.Title);
                    break;
                case "createdat":
                    if (request.SortDescending)
                        spec.ApplyOrderByDescending(e => e.CreatedAt);
                    else
                        spec.ApplyOrderBy(e => e.CreatedAt);
                    break;
                default: // StartDate
                    if (request.SortDescending)
                        spec.ApplyOrderByDescending(e => e.EventDates.StartDate);
                    else
                        spec.ApplyOrderBy(e => e.EventDates.StartDate);
                    break;
            }

            var events = await _eventRepository.ListBySpecAsync(spec, cancellationToken);
            var totalCount = await _eventRepository.CountAsync(spec, cancellationToken);

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

            var result = new PaginatedResult<EventSummaryDto>(eventDtos, totalCount, request.PageNumber, request.PageSize);
            return Result<PaginatedResult<EventSummaryDto>>.Success(result);
        }
    }
}
