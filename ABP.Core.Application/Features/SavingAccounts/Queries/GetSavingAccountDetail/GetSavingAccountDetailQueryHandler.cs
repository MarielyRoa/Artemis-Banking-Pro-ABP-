using ABP.Core.Application.Dtos.SavingAccounts;
using ABP.Core.Domain.Interfaces;
using AutoMapper;
using MediatR;


namespace ABP.Core.Application.Features.SavingAccounts.Queries.GetSavingAccountDetail
{
    public class GetSavingAccountDetailQueryHandler : IRequestHandler<GetSavingAccountDetailQuery, SavingAccountDetailDto?>
    {
        private readonly ISavingAccountRepository _savingAccountRepository;
        private readonly IMapper _mapper;

        public GetSavingAccountDetailQueryHandler(ISavingAccountRepository savingAccountRepository, IMapper mapper)
        {
            _savingAccountRepository = savingAccountRepository;
            _mapper = mapper;
        }

        public async Task<SavingAccountDetailDto?> Handle(GetSavingAccountDetailQuery request, CancellationToken cancellationToken)
        {
            var account = await _savingAccountRepository.GetByAccountNumberAsync(request.AccountNumber);
            return account == null ? null : _mapper.Map<SavingAccountDetailDto>(account);
        }
    }
}
