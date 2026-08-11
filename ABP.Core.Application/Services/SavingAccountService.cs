using ABP.Core.Application.Dtos.SavingAccounts;
using ABP.Core.Application.Interfaces;
using ABP.Core.Domain.Entities;
using ABP.Core.Domain.Interfaces;
using AutoMapper;


namespace ABP.Core.Application.Services
{
    public class SavingAccountService : GenericService<SavingAccount, SavingAccountDto>, ISavingAccountService
    {
        private readonly ISavingAccountRepository _savingAccountRepository;
        private readonly IMapper _mapper;

        public SavingAccountService(ISavingAccountRepository savingAccountRepository, IMapper mapper)
            : base(savingAccountRepository, mapper)
        {
            _savingAccountRepository = savingAccountRepository;
            _mapper = mapper;
        }

        public async Task<SavingAccountDto?> GetByAccountNumberAsync(string accountNumber)
        {
            var account = await _savingAccountRepository.GetByAccountNumberAsync(accountNumber);
            return account == null ? null : _mapper.Map<SavingAccountDto>(account);
        }

        public async Task<List<SavingAccountDto>> GetAllByClientIdAsync(string clientId)
        {
            var accounts = await _savingAccountRepository.GetAllByClientIdAsync(clientId);
            return _mapper.Map<List<SavingAccountDto>>(accounts);
        }

        public async Task<SavingAccountDto?> GetPrincipalAccountByClientIdAsync(string clientId)
        {
            var account = await _savingAccountRepository.GetPrincipalAccountByClientIdAsync(clientId);
            return account == null ? null : _mapper.Map<SavingAccountDto>(account);
        }

        public async Task<bool> ExistsAccountNumberAsync(string accountNumber)
        {
            return await _savingAccountRepository.ExistsAccountNumberAsync(accountNumber);
        }
    }
}
