using FluentValidation;
using Ordering.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Validators
{
    class CheckoutOrdeCommandValidatorV2 : AbstractValidator<CheckoutOrderCommandV2>
    {
        public CheckoutOrdeCommandValidatorV2()
        {
            RuleFor(o => o.UserName)
                .NotEmpty()
                .WithMessage("{UserName} is required")
                .NotNull()
                .MaximumLength(70)
                .WithMessage("{Username} must not exceed 70 characters");

            RuleFor(o => o.TotalPrice)
                    .NotEmpty()
                    .WithMessage("{TotalPrice} is required")
                    .GreaterThan(-1)
                    .WithMessage("{TotalPrice} should not be -");
        }

    }
}
