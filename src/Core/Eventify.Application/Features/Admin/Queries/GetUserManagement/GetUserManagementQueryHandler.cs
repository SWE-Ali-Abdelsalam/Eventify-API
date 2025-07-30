using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Features.Admin.DTOs;
using Eventify.Domain.Entities.Payments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Eventify.Application.Features.Admin.Queries.GetUserManagement
{
    public class GetUserManagementQueryHandler : IRequestHandler<GetUserManagementQuery, Result<PaginatedResult<UserManagementDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetUserManagementQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedResult<UserManagementDto>>> Handle(GetUserManagementQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Users.AsQueryable();

                // Apply filters
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    query = query.Where(u =>
                        u.FirstName.ToLower().Contains(searchTerm) ||
                        u.LastName.ToLower().Contains(searchTerm) ||
                        u.Email.ToLower().Contains(searchTerm));
                }

                if (request.IsActiveFilter.HasValue)
                {
                    query = query.Where(u => u.IsActive == request.IsActiveFilter.Value);
                }

                if (request.IsVerifiedFilter.HasValue)
                {
                    query = query.Where(u => u.IsEmailVerified == request.IsVerifiedFilter.Value);
                }

                if (request.RoleFilter.HasValue)
                {
                    query = query.Where(u => u.UserRoles.Any(ur => ur.Role == request.RoleFilter.Value && ur.IsActive));
                }

                // Apply sorting
                query = request.SortBy?.ToLower() switch
                {
                    "firstname" => request.SortDescending
                        ? query.OrderByDescending(u => u.FirstName)
                        : query.OrderBy(u => u.FirstName),
                    "lastname" => request.SortDescending
                        ? query.OrderByDescending(u => u.LastName)
                        : query.OrderBy(u => u.LastName),
                    "email" => request.SortDescending
                        ? query.OrderByDescending(u => u.Email)
                        : query.OrderBy(u => u.Email),
                    "lastloginat" => request.SortDescending
                        ? query.OrderByDescending(u => u.LastLoginAt)
                        : query.OrderBy(u => u.LastLoginAt),
                    _ => request.SortDescending
                        ? query.OrderByDescending(u => u.CreatedAt)
                        : query.OrderBy(u => u.CreatedAt)
                };

                // Get total count
                var totalCount = await query.CountAsync(cancellationToken);

                // Apply pagination
                var users = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Include(u => u.UserRoles)
                    .Include(u => u.Bookings)
                    .Include(u => u.OrganizedEvents)
                    .ToListAsync(cancellationToken);

                // Get payment information for users
                var userIds = users.Select(u => u.Id).ToList();
                var userPayments = await _context.Payments
                    .Where(p => userIds.Contains(p.Booking.UserId) && p.Status == PaymentStatus.Completed)
                    .GroupBy(p => p.Booking.UserId)
                    .Select(g => new { UserId = g.Key, TotalSpent = g.Sum(p => p.Amount.Amount) })
                    .ToListAsync(cancellationToken);

                var paymentLookup = userPayments.ToDictionary(up => up.UserId, up => up.TotalSpent);

                // Map to DTOs
                var userDtos = users.Select(u => new UserManagementDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    IsActive = u.IsActive,
                    IsEmailVerified = u.IsEmailVerified,
                    IsPhoneVerified = u.IsPhoneVerified,
                    CreatedAt = u.CreatedAt,
                    LastLoginAt = u.LastLoginAt,
                    TotalBookings = u.Bookings.Count,
                    TotalEvents = u.OrganizedEvents.Count,
                    TotalSpent = paymentLookup.GetValueOrDefault(u.Id, 0),
                    Roles = u.UserRoles.Select(ur => new UserRoleDto
                    {
                        Id = ur.Id,
                        Role = ur.Role.ToString(),
                        AssignedAt = ur.AssignedAt,
                        AssignedBy = ur.AssignedBy,
                        ExpiresAt = ur.ExpiresAt,
                        IsActive = ur.IsActive,
                        IsValid = ur.IsValid
                    }).ToList()
                }).ToList();

                var result = new PaginatedResult<UserManagementDto>(
                    userDtos,
                    totalCount,
                    request.PageNumber,
                    request.PageSize
                );

                return Result<PaginatedResult<UserManagementDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<UserManagementDto>>.Failure($"Error retrieving user management data: {ex.Message}");
            }
        }
    }
}
