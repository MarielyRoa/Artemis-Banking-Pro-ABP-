using FluentValidation;


namespace ABP.Core.Application.Features.CreditCards.Commands.CreateCreditCard
{
    public class CreateCreditCardCommandValidator : AbstractValidator<CreateCreditCardCommand>
    {
        public CreateCreditCardCommandValidator()
        {
            RuleFor(x => x.ClientId).NotEmpty().WithMessage("El cliente es obligatorio.");
            RuleFor(x => x.AssignedByUserId).NotEmpty().WithMessage("El administrador responsable es obligatorio.");
            RuleFor(x => x.CreditLimit).GreaterThan(0).WithMessage("El límite de crédito debe ser mayor a 0.");
        }
    }
}
