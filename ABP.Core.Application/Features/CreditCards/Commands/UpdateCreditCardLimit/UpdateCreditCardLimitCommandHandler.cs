using ABP.Core.Domain.Common.Enums;
using ABP.Core.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Core.Application.Features.CreditCards.Commands.UpdateCreditCardLimit
{
    public class UpdateCreditCardLimitCommandHandler : IRequestHandler<UpdateCreditCardLimitCommand, bool>
    {
        private readonly ICreditCardRepository _creditCardRepository;

        public UpdateCreditCardLimitCommandHandler(ICreditCardRepository creditCardRepository)
        {
            _creditCardRepository = creditCardRepository;
        }

        public async Task<bool> Handle(UpdateCreditCardLimitCommand request, CancellationToken cancellationToken)
        {
            var card = await _creditCardRepository.GetByIdAsync(request.CreditCardId);
            if (card == null)
                throw new InvalidOperationException("La tarjeta no existe.");

            if (card.Status != CreditCardStatus.Active)
                throw new InvalidOperationException("Solo se puede editar el límite de una tarjeta activa.");

            // Regla clave: el nuevo límite no puede ser menor a la deuda actual
            if (request.NewCreditLimit < card.CurrentDebt)
                throw new InvalidOperationException(
                    $"El nuevo límite (${request.NewCreditLimit}) no puede ser menor a la deuda actual (${card.CurrentDebt}).");

            card.CreditLimit = request.NewCreditLimit;
            await _creditCardRepository.UpdateAsync(card.Id, card);

            return true;
        }
    }
}
