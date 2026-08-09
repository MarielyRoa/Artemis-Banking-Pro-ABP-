using System.Threading.Tasks;
using ABP.Core.Application.Dtos;
using ABP.Core.Application.Dtos.Cashier;

namespace ABP.Core.Application.Services
{
    public interface ICashierService
    {
        Task<DashboardDto> GetDashboardAsync(string cashierUserId);
        Task<int> DepositAsync(DepositDto dto);
        Task<int> WithdrawAsync(WithdrawalDto dto);
        Task<int> PayCreditCardAsync(CreditCardPaymentDto dto);
        Task<int> PayLoanAsync(LoanPaymentDto dto);
        Task<int> TransferToThirdPartyAsync(ThirdPartyTransferDto dto);
        Task<OperationConfirmationDto> GetConfirmationAsync(string operationId);
        Task<OperationHistoryDto> GetHistoryAsync(string cashierUserId);
    }
}
