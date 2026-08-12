using ABP.Core.Application.Dtos.CardTransactions;


namespace ABP.Core.Application.Dtos.CreditCards
{
    public class CreditCardDetailDto : CreditCardDto
    {
        public List<CardTransactionDto> Consumptions { get; set; } = new();
    }
}
