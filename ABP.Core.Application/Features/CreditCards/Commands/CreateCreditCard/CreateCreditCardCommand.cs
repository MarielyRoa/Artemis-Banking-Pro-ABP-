using ABP.Core.Application.Dtos.CreditCards;
using MediatR;


namespace ABP.Core.Application.Features.CreditCards.Commands.CreateCreditCard
{
    public class CreateCreditCardCommand : IRequest<CreditCardDto>
    {
        public required string ClientId { get; set; }
        public required string AssignedByUserId { get; set; }
        public decimal CreditLimit { get; set; }
    }
}
