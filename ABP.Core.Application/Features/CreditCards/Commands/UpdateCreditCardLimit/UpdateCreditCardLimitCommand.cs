using MediatR;


namespace ABP.Core.Application.Features.CreditCards.Commands.UpdateCreditCardLimit
{
    public class UpdateCreditCardLimitCommand : IRequest<bool>
    {
        public required int CreditCardId { get; set; }
        public decimal NewCreditLimit { get; set; }
    }
}
