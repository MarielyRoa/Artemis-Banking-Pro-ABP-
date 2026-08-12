using ABP.Core.Application.Dtos.Transactions;


namespace ABP.Core.Application.Dtos.SavingAccounts
{
    public class SavingAccountDetailDto : SavingAccountDto
    {
        public List<TransactionDto> Transactions { get; set; } = new();
    }
}
