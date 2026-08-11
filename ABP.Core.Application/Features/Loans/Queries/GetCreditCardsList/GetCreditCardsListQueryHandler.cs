using ABP.Core.Application.Dtos.Common;
using ABP.Core.Application.Dtos.CreditCards;
using ABP.Core.Domain.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace ABP.Core.Application.Features.Loans.Queries.GetCreditCardsList
{
    public class GetCreditCardsListQueryHandler : IRequestHandler<GetCreditCardsListQuery, PagedResult<CreditCardDto>>
    {
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IMapper _mapper;

        public GetCreditCardsListQueryHandler(ICreditCardRepository creditCardRepository, IMapper mapper)
        {
            _creditCardRepository = creditCardRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<CreditCardDto>> Handle(GetCreditCardsListQuery request, CancellationToken cancellationToken)
        {
            var query = _creditCardRepository.GetAllQuery();

            if (request.Status.HasValue)
                query = query.Where(c => c.Status == request.Status.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<CreditCardDto>>(items);
            dtos.ForEach(d => d.Cvc = "***"); // nunca exponer el CVC/hash en listados

            return new PagedResult<CreditCardDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
