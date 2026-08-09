namespace ABP.Core.Application.Dtos
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Beneficiary { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
