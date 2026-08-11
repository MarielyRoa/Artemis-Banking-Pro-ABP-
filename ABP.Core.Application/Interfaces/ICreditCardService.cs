using ABP.Core.Application.Dtos.CreditCards;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ABP.Core.Application.Interfaces
{
    public interface ICreditCardService : IGenericService<CreditCardDto>
    {
        Task<CreditCardDto?> GetByCardNumberAsync(string cardNumber);
        Task<List<CreditCardDto>> GetAllByClientIdAsync(string clientId);
        Task<bool> ExistsCardNumberAsync(string cardNumber);
    }
}
