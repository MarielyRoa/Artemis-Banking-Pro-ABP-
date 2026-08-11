using ABP.Core.Application.Dtos.CreditCards;
using ABP.Core.Application.Interfaces;
using ABP.Core.Domain.Entities;
using ABP.Core.Domain.Interfaces;
using AutoMapper;


namespace ABP.Core.Application.Services
{
    public class CreditCardService : GenericService<CreditCard, CreditCardDto>, ICreditCardService
    {
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IMapper _mapper;

        public CreditCardService(ICreditCardRepository creditCardRepository, IMapper mapper)
            : base(creditCardRepository, mapper)
        {
            _creditCardRepository = creditCardRepository;
            _mapper = mapper;
        }

        public async Task<CreditCardDto?> GetByCardNumberAsync(string cardNumber)
        {
            var card = await _creditCardRepository.GetByCardNumberAsync(cardNumber);
            return card == null ? null : _mapper.Map<CreditCardDto>(card);
        }

        public async Task<List<CreditCardDto>> GetAllByClientIdAsync(string clientId)
        {
            var cards = await _creditCardRepository.GetAllByClientIdAsync(clientId);
            return _mapper.Map<List<CreditCardDto>>(cards);
        }

        public async Task<bool> ExistsCardNumberAsync(string cardNumber)
        {
            return await _creditCardRepository.ExistsCardNumberAsync(cardNumber);
        }
    }
}
