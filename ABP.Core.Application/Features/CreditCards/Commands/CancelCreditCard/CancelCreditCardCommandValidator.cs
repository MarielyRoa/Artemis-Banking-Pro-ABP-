using FluentValidation;

namespace ABP.Core.Application.Features.CreditCards.Commands.CancelCreditCard
{
    public class CancelCreditCardCommandValidator : AbstractValidator<CancelCreditCardCommand>
    {
        public CancelCreditCardCommandValidator()
        {
            RuleFor(x => x.CreditCardId).GreaterThan(0).WithMessage("La tarjeta es obligatoria.");
        }
    }
}
