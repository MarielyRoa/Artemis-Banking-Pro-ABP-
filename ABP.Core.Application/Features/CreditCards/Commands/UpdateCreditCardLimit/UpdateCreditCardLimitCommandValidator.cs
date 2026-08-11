using FluentValidation;

namespace ABP.Core.Application.Features.CreditCards.Commands.UpdateCreditCardLimit
{
    public class UpdateCreditCardLimitCommandValidator : AbstractValidator<UpdateCreditCardLimitCommand>
    {
        public UpdateCreditCardLimitCommandValidator()
        {
            RuleFor(x => x.CreditCardId).GreaterThan(0).WithMessage("La tarjeta es obligatoria.");
            RuleFor(x => x.NewCreditLimit).GreaterThan(0).WithMessage("El límite debe ser mayor a 0.");
        }
    }


}
