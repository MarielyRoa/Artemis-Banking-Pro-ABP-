using ABP.Core.Domain.Common.Enums;
using ABP.Core.Domain.Interfaces;
using MediatR;


namespace ABP.Core.Application.Features.CreditCards.Commands.CancelCreditCard
{
    public class CancelCreditCardCommandHandler : IRequestHandler<CancelCreditCardCommand, bool>
    {
        private readonly ICreditCardRepository _creditCardRepository;

        public CancelCreditCardCommandHandler(ICreditCardRepository creditCardRepository)
        {
            _creditCardRepository = creditCardRepository;
        }

        public async Task<bool> Handle(CancelCreditCardCommand request, CancellationToken cancellationToken)
        {
            var card = await _creditCardRepository.GetByIdAsync(request.CreditCardId);
            if (card == null)
                throw new InvalidOperationException("La tarjeta no existe.");

            if (card.Status != CreditCardStatus.Active)
                throw new InvalidOperationException("La tarjeta ya está cancelada.");

            // Regla clave: solo se puede cancelar si no tiene deuda pendiente
            if (card.CurrentDebt > 0)
                throw new InvalidOperationException("No se puede cancelar la tarjeta: tiene deuda pendiente.");

            card.Status = CreditCardStatus.Cancelled;
            await _creditCardRepository.UpdateAsync(card.Id, card);

            return true;
        }
    }
}
