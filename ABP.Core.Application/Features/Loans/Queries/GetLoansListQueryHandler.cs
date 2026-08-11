using ABP.Core.Application.Dtos.Common;
using ABP.Core.Application.Dtos.Loans;
using ABP.Core.Domain.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;



namespace ABP.Core.Application.Features.Loans.Queries
{
    public class GetLoansListQueryHandler : IRequestHandler<GetLoansListQuery, PagedResult<LoanDto>>
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IMapper _mapper;

        public GetLoansListQueryHandler(ILoanRepository loanRepository, IMapper mapper)
        {
            _loanRepository = loanRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<LoanDto>> Handle(GetLoansListQuery request, CancellationToken cancellationToken)
        {
            var query = _loanRepository.GetAllQuery();

            if (request.Status.HasValue)
                query = query.Where(l => l.Status == request.Status.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<LoanDto>
            {
                Items = _mapper.Map<List<LoanDto>>(items),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
