using System.ComponentModel.DataAnnotations;

namespace ABP.Core.Application.Dtos
{
    public class WithdrawalDto
    {
        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string SourceAccountNumber { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }
    }
}
