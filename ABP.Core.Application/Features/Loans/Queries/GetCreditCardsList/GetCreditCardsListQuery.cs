using ABP.Core.Application.Dtos.Common;
using ABP.Core.Application.Dtos.CreditCards;
using ABP.Core.Domain.Common.Enums;
using MediatR;


namespace ABP.Core.Application.Features.Loans.Queries.GetCreditCardsList
{
    public class GetCreditCardsListQuery : IRequest<PagedResult<CreditCardDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public CreditCardStatus? Status { get; set; }
    }
}
