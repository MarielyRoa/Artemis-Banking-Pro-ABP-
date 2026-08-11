using ABP.Core.Application.Dtos.Common;
using ABP.Core.Application.Dtos.SavingAccounts;
using ABP.Core.Domain.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ABP.Core.Application.Features.Loans.Queries.GetSavingAccountsList
{
    public class GetSavingAccountsListQueryHandler : IRequestHandler<GetSavingAccountsListQuery, PagedResult<SavingAccountDto>>
    {
        private readonly ISavingAccountRepository _savingAccountRepository;
        private readonly IMapper _mapper;

        public GetSavingAccountsListQueryHandler(ISavingAccountRepository savingAccountRepository, IMapper mapper)
        {
            _savingAccountRepository = savingAccountRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<SavingAccountDto>> Handle(GetSavingAccountsListQuery request, CancellationToken cancellationToken)
        {
            var query = _savingAccountRepository.GetAllQuery();

            if (request.Status.HasValue)
                query = query.Where(s => s.Status == request.Status.Value);

            if (request.AccountType.HasValue)
                query = query.Where(s => s.AccountType == request.AccountType.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<SavingAccountDto>
            {
                Items = _mapper.Map<List<SavingAccountDto>>(items),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }

}
