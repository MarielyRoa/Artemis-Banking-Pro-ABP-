using ABP.Core.Application.Dtos.Loans;
using ABP.Core.Domain.Interfaces;
using AutoMapper;
using MediatR;


namespace ABP.Core.Application.Features.Loans.Queries.GetLoanDetail
{
    public class GetLoanDetailQueryHandler : IRequestHandler<GetLoanDetailQuery, LoanDetailDto?>
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IMapper _mapper;

        public GetLoanDetailQueryHandler(ILoanRepository loanRepository, IMapper mapper)
        {
            _loanRepository = loanRepository;
            _mapper = mapper;
        }

        public async Task<LoanDetailDto?> Handle(GetLoanDetailQuery request, CancellationToken cancellationToken)
        {
            var loan = await _loanRepository.GetByLoanNumberAsync(request.LoanNumber);
            return loan == null ? null : _mapper.Map<LoanDetailDto>(loan);
        }
    }
}
