using Eventify.Application.Common;
using Eventify.Application.Features.Events.DTOs;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Events;
using Eventify.Domain.Specifications.EventSpecifications;
using MediatR;

namespace Eventify.Application.Features.Events.Queries.GetEvent
{
    public class GetEventQueryHandler : IRequestHandler<GetEventQuery, Result<EventDetailDto>>
    {
        private readonly IRepository<Event> _eventRepository;

        public GetEventQueryHandler(IRepository<Event> eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<Result<EventDetailDto>> Handle(GetEventQuery request, CancellationToken cancellationToken)
        {
            var spec = new EventWithDetailsSpecification(request.EventId);
            var eventEntity = await _eventRepository.GetBySpecAsync(spec, cancellationToken);

            if (eventEntity == null)
            {
                return Result<EventDetailDto>.Failure("Event not found");
            }

            var eventDto = new EventDetailDto
            {
                Id = eventEntity.Id,
                Title = eventEntity.Title,
                Description = eventEntity.Description,
                ShortDescription = eventEntity.ShortDescription,
                StartDate = eventEntity.EventDates.StartDate,
                EndDate = eventEntity.EventDates.EndDate,
                Status = eventEntity.Status.ToString(),
                IsPublic = eventEntity.IsPublic,
                IsVirtual = eventEntity.IsVirtual,
                VirtualMeetingUrl = eventEntity.VirtualMeetingUrl,
                MaxCapacity = eventEntity.MaxCapacity,
                CurrentRegistrations = eventEntity.CurrentRegistrations,
                AvailableSpots = eventEntity.AvailableSpots,
                RequiresApproval = eventEntity.RequiresApproval,
                RegistrationOpenDate = eventEntity.RegistrationOpenDate,
                RegistrationCloseDate = eventEntity.RegistrationCloseDate,
                IsRegistrationOpen = eventEntity.IsRegistrationOpen,
                ImageUrl = eventEntity.ImageUrl,
                WebsiteUrl = eventEntity.WebsiteUrl,
                Tags = eventEntity.Tags,
                CreatedAt = eventEntity.CreatedAt,
                Category = new EventCategoryDto
                {
                    Id = eventEntity.Category.Id,
                    Name = eventEntity.Category.Name,
                    Color = eventEntity.Category.Color,
                    IconUrl = eventEntity.Category.IconUrl
                },
                Organizer = new EventOrganizerDto
                {
                    Id = eventEntity.Organizer.Id,
                    Name = eventEntity.Organizer.FullName,
                    Email = eventEntity.Organizer.Email
                },
                Venue = eventEntity.Venue != null ? new VenueDetailDto
                {
                    Id = eventEntity.Venue.Id,
                    Name = eventEntity.Venue.Name,
                    Description = eventEntity.Venue.Description,
                    Street = eventEntity.Venue.Address.Street,
                    City = eventEntity.Venue.Address.City,
                    State = eventEntity.Venue.Address.State,
                    Country = eventEntity.Venue.Address.Country,
                    PostalCode = eventEntity.Venue.Address.PostalCode,
                    Latitude = eventEntity.Venue.Address.Latitude,
                    Longitude = eventEntity.Venue.Address.Longitude,
                    Capacity = eventEntity.Venue.Capacity,
                    ImageUrl = eventEntity.Venue.ImageUrl
                } : null,
                Sessions = eventEntity.Sessions.Select(s => new EventSessionDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    StartTime = s.SessionDates.StartDate,
                    EndTime = s.SessionDates.EndDate,
                    Location = s.Location,
                    SpeakerName = s.SpeakerName,
                    SpeakerBio = s.SpeakerBio,
                    MaxCapacity = s.MaxCapacity,
                    CurrentRegistrations = s.CurrentRegistrations,
                    IsRequired = s.IsRequired
                }).ToList(),
                TicketTypes = eventEntity.TicketTypes.Select(tt => new TicketTypeDetailDto
                {
                    Id = tt.Id,
                    Name = tt.Name,
                    Description = tt.Description,
                    Price = tt.Price.Amount,
                    Currency = tt.Price.Currency,
                    Quantity = tt.Quantity,
                    SoldQuantity = tt.SoldQuantity,
                    AvailableQuantity = tt.AvailableQuantity,
                    SaleStartDate = tt.SaleStartDate,
                    SaleEndDate = tt.SaleEndDate,
                    MaxPerOrder = tt.MaxPerOrder,
                    MinPerOrder = tt.MinPerOrder,
                    IsAvailable = tt.IsAvailable,
                    IsOnSale = tt.IsOnSale,
                    Terms = tt.Terms
                }).ToList(),
                Images = eventEntity.Images.Select(i => new EventImageDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    Caption = i.Caption,
                    IsPrimary = i.IsPrimary
                }).ToList(),
                Documents = eventEntity.Documents.Where(d => d.IsPublic).Select(d => new EventDocumentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    FileUrl = d.FileUrl,
                    IsPublic = d.IsPublic
                }).ToList()
            };

            return Result<EventDetailDto>.Success(eventDto);
        }
    }
}
