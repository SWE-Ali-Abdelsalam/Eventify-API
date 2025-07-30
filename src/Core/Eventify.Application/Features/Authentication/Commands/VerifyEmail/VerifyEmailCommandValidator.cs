﻿using FluentValidation;

namespace Eventify.Application.Features.Authentication.Commands.VerifyEmail
{
    public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
    {
        public VerifyEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format");

            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("Verification token is required");
        }
    }
}
