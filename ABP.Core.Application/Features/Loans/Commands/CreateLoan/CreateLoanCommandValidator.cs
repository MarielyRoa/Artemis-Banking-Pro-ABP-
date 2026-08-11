using FluentValidation;


namespace ABP.Core.Application.Features.Loans.Commands.CreateLoan
{
    public class CreateLoanCommandValidator : AbstractValidator<CreateLoanCommand>
    {
        public CreateLoanCommandValidator()
        {
            RuleFor(x => x.ClientId).NotEmpty().WithMessage("El cliente es obligatorio.");
            RuleFor(x => x.AssignedByUserId).NotEmpty().WithMessage("El administrador responsable es obligatorio.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("El monto del préstamo debe ser mayor a 0.");
            RuleFor(x => x.AnnualInterestRate).GreaterThanOrEqualTo(0).WithMessage("La tasa de interés no puede ser negativa.");
            RuleFor(x => x.TermInMonths).GreaterThan(0).WithMessage("El plazo debe ser de al menos 1 mes.");
        }
    }
}
