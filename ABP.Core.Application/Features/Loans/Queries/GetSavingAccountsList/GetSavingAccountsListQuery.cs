using ABP.Core.Application.Dtos.Common;
using ABP.Core.Application.Dtos.SavingAccounts;
using ABP.Core.Domain.Common.Enums;
using MediatR;


namespace ABP.Core.Application.Features.Loans.Queries.GetSavingAccountsList
{
    public class GetSavingAccountsListQuery : IRequest<PagedResult<SavingAccountDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public SavingAccountStatus? Status { get; set; }
        public SavingAccountType? AccountType { get; set; }
    }
}
