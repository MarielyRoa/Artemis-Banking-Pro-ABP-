using ABP.Core.Application.Dtos.CreditCards;
using ABP.Core.Domain.Interfaces;
using AutoMapper;
using MediatR;


namespace ABP.Core.Application.Features.CreditCards.Queries.GetCreditCardDetail
{
    public class GetCreditCardDetailQueryHandler : IRequestHandler<GetCreditCardDetailQuery, CreditCardDetailDto?>
    {
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IMapper _mapper;

        public GetCreditCardDetailQueryHandler(ICreditCardRepository creditCardRepository, IMapper mapper)
        {
            _creditCardRepository = creditCardRepository;
            _mapper = mapper;
        }

        public async Task<CreditCardDetailDto?> Handle(GetCreditCardDetailQuery request, CancellationToken cancellationToken)
        {
            var card = await _creditCardRepository.GetByCardNumberAsync(request.CardNumber);
            if (card == null) return null;

            var dto = _mapper.Map<CreditCardDetailDto>(card);
            dto.Cvc = "***"; // nunca exponer el CVC/hash, ni en el detalle
            return dto;
        }
    }
}
