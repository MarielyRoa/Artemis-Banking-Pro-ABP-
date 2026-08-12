using ABP.Core.Application.Dtos.SavingAccounts;
using MediatR;


namespace ABP.Core.Application.Features.SavingAccounts.Queries.GetSavingAccountDetail
{
    public class GetSavingAccountDetailQuery : IRequest<SavingAccountDetailDto?>
    {
        public required string AccountNumber { get; set; }
    }
}
