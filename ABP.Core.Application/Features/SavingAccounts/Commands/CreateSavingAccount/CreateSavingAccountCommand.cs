using ABP.Core.Application.Dtos.SavingAccounts;
using ABP.Core.Domain.Common.Enums;
using MediatR;


namespace ABP.Core.Application.Features.SavingAccounts.Commands.CreateSavingAccount
{
    public class CreateSavingAccountCommand : IRequest<SavingAccountDto>
    {
        public required string ClientId { get; set; }
        public required SavingAccountType AccountType { get; set; }
        public decimal InitialBalance { get; set; } = 0;
    }
}
