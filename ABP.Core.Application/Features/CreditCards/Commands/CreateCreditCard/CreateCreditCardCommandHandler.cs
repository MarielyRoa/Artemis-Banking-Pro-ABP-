using ABP.Core.Application.Dtos.CreditCards;
using ABP.Core.Domain.Common.Enums;
using ABP.Core.Domain.Entities;
using ABP.Core.Domain.Interfaces;
using AutoMapper;
using MediatR;


namespace ABP.Core.Application.Features.CreditCards.Commands.CreateCreditCard
{
    public class CreateCreditCardCommandHandler : IRequestHandler<CreateCreditCardCommand, CreditCardDto>
    {
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly ISavingAccountRepository _savingAccountRepository;
        private readonly IMapper _mapper;

        public CreateCreditCardCommandHandler(
            ICreditCardRepository creditCardRepository,
            ISavingAccountRepository savingAccountRepository,
            IMapper mapper)
        {
            _creditCardRepository = creditCardRepository;
            _savingAccountRepository = savingAccountRepository;
            _mapper = mapper;
        }

        public async Task<CreditCardDto> Handle(CreateCreditCardCommand request, CancellationToken cancellationToken)
        {
            // Solo a cliente activo: lo confirmamos verificando que tenga cuenta principal activa
            var principalAccount = await _savingAccountRepository.GetPrincipalAccountByClientIdAsync(request.ClientId);
            if (principalAccount == null)
                throw new InvalidOperationException("El cliente no tiene una cuenta principal activa.");

            var cardNumber = await GenerateUniqueCardNumberAsync();
            var expirationDate = DateTime.UtcNow.AddYears(4).ToString("MM/yy");

            // Genera CVC de 3 dígitos, lo hashea, y NUNCA se guarda ni se retorna en texto plano
            var rawCvc = new Random().Next(100, 999).ToString();
            var hashedCvc = BCrypt.Net.BCrypt.HashPassword(rawCvc);

            var card = new CreditCard
            {
                Id = 0,
                CardNumber = cardNumber,
                ClientId = request.ClientId,
                CreditLimit = request.CreditLimit,
                CurrentDebt = 0,
                ExpirationDate = expirationDate,
                Cvc = hashedCvc,
                Status = CreditCardStatus.Active,
                AssignedByUserId = request.AssignedByUserId
            };

            var createdCard = await _creditCardRepository.AddAsync(card);

            // TODO (Persona2): enviar correo de notificación al cliente con los últimos 4 dígitos
            // cuando el servicio de correo esté disponible (Infrastructure.Shared / MailSettings).

            var dto = _mapper.Map<CreditCardDto>(createdCard);
            dto.Cvc = "***"; // nunca se expone el hash ni el CVC real en la respuesta
            return dto;
        }

        private async Task<string> GenerateUniqueCardNumberAsync()
        {
            string cardNumber;
            bool exists;
            var random = new Random();
            do
            {
                cardNumber = string.Concat(Enumerable.Range(0, 16).Select(_ => random.Next(0, 10).ToString()));
                exists = await _creditCardRepository.ExistsCardNumberAsync(cardNumber);
            } while (exists);

            return cardNumber;
        }
    }
}
