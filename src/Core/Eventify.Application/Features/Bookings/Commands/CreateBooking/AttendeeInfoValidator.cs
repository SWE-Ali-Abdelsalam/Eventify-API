﻿using Eventify.Application.Features.Bookings.DTOs;
using FluentValidation;

namespace Eventify.Application.Features.Bookings.Commands.CreateBooking
{
    public class AttendeeInfoValidator : AbstractValidator<AttendeeInfo>
    {
        public AttendeeInfoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required")
                .MaximumLength(100)
                .WithMessage("First name cannot exceed 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required")
                .MaximumLength(100)
                .WithMessage("Last name cannot exceed 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format");

            RuleFor(x => x.Phone)
                .Matches(@"^\+?[\d\s\-\(\)]+$")
                .WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrEmpty(x.Phone));
        }
    }
}
