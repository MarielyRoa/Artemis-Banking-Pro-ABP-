using MediatR;


namespace ABP.Core.Application.Features.CreditCards.Commands.UpdateLoanRate
{
    public class UpdateLoanRateCommand : IRequest<bool>
    {
        public required int LoanId { get; set; }
        public decimal NewAnnualInterestRate { get; set; }
    }
}
