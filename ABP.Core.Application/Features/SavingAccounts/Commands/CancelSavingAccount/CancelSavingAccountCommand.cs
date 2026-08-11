using MediatR;


namespace ABP.Core.Application.Features.SavingAccounts.Commands.CancelSavingAccount
{
    public class CancelSavingAccountCommand : IRequest<bool>
    {
        public required int SavingAccountId { get; set; }
    }
}
