using FluentValidation;


namespace ABP.Core.Application.Features.SavingAccounts.Commands.CancelSavingAccount
{
    public class CancelSavingAccountCommandValidator : AbstractValidator<CancelSavingAccountCommand>
    {
        public CancelSavingAccountCommandValidator()
        {
            RuleFor(x => x.SavingAccountId)
                .GreaterThan(0).WithMessage("La cuenta es obligatoria.");
        }
    }
}
