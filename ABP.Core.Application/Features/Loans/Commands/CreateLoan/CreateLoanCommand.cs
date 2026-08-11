using ABP.Core.Application.Dtos.Loans;
using MediatR;


namespace ABP.Core.Application.Features.Loans.Commands.CreateLoan
{
    public class CreateLoanCommand : IRequest<LoanDto>
    {
        public required string ClientId { get; set; }
        public required string AssignedByUserId { get; set; } // Admin autenticado que asigna el préstamo
        public decimal Amount { get; set; }
        public decimal AnnualInterestRate { get; set; }
        public int TermInMonths { get; set; }
    }
}
