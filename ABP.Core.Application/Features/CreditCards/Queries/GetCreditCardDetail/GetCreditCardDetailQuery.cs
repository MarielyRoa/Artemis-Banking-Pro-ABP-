using ABP.Core.Application.Dtos.CreditCards;
using MediatR;

namespace ABP.Core.Application.Features.CreditCards.Queries.GetCreditCardDetail
{
    public class GetCreditCardDetailQuery : IRequest<CreditCardDetailDto?>
    {
        public required string CardNumber { get; set; }
    }
}
