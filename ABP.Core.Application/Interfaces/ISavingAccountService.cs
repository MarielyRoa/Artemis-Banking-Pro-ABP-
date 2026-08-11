using ABP.Core.Application.Dtos.SavingAccounts;

namespace ABP.Core.Application.Interfaces
{
    public interface ISavingAccountService : IGenericService<SavingAccountDto>
    {
        Task<SavingAccountDto?> GetByAccountNumberAsync(string accountNumber);
        Task<List<SavingAccountDto>> GetAllByClientIdAsync(string clientId);
        Task<SavingAccountDto?> GetPrincipalAccountByClientIdAsync(string clientId);
        Task<bool> ExistsAccountNumberAsync(string accountNumber);
    }
}
