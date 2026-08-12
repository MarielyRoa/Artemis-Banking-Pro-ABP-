using ABP.Core.Application.Dtos.Loans;
using MediatR;


namespace ABP.Core.Application.Features.Loans.Queries.GetLoanDetail
{
    public class GetLoanDetailQuery : IRequest<LoanDetailDto?>
    {
        public required string LoanNumber { get; set; }
    }
}
