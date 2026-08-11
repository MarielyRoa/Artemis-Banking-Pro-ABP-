using FluentValidation;


namespace ABP.Core.Application.Features.SavingAccounts.Commands.CreateSavingAccount
{
    public class CreateSavingAccountCommandValidator : AbstractValidator<CreateSavingAccountCommand>
    {
        public CreateSavingAccountCommandValidator()
        {
            RuleFor(x => x.ClientId)
                .NotEmpty().WithMessage("El cliente es obligatorio.");

            RuleFor(x => x.InitialBalance)
                .GreaterThanOrEqualTo(0).WithMessage("El monto inicial no puede ser negativo.");
        }
    }
}
