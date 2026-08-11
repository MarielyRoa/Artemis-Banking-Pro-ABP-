using FluentValidation;

namespace ABP.Core.Application.Features.CreditCards.Commands.UpdateLoanRate
{
    public class UpdateLoanRateCommandValidator : AbstractValidator<UpdateLoanRateCommand>
    {
        public UpdateLoanRateCommandValidator()
        {
            RuleFor(x => x.LoanId).GreaterThan(0).WithMessage("El préstamo es obligatorio.");
            RuleFor(x => x.NewAnnualInterestRate).GreaterThanOrEqualTo(0).WithMessage("La tasa no puede ser negativa.");
        }
    }
}
