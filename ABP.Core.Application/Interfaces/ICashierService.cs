using System.Threading.Tasks;
using ABP.Core.Application.Dtos;
using System.Collections.Generic;

namespace ABP.Core.Application.Interfaces
{
    public interface ICashierService
    {
        Task<int> DepositAsync(DepositDto depositDto);
        Task<int> WithdrawAsync(WithdrawalDto withdrawalDto);
        Task<int> PayCreditCardAsync(string creditCardNumber, decimal amount);
        Task<int> PayLoanAsync(string loanNumber, decimal amount);
        Task<int> TransferToThirdPartyAsync(string destinationAccountNumber, decimal amount);
        Task<DashboardDto> GetDashboardAsync(string cashierUserId);
        Task<IReadOnlyList<TransactionDto>> GetTransactionHistoryAsync(string cashierUserId, int page = 1, int pageSize = 20);
    }
}
