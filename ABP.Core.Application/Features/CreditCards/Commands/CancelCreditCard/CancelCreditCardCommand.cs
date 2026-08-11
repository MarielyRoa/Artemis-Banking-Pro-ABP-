using MediatR;


namespace ABP.Core.Application.Features.CreditCards.Commands.CancelCreditCard
{
    public class CancelCreditCardCommand : IRequest<bool>
    {
        public required int CreditCardId { get; set; }
    }
}
