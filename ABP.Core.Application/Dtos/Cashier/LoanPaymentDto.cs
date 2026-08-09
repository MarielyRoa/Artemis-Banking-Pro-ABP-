using System.ComponentModel.DataAnnotations;

namespace ABP.Core.Application.Dtos.Cashier
{
    public class LoanPaymentDto
    {
        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string LoanNumber { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero.")]
        public decimal Amount { get; set; }

        // Opcional: identificar al cajero que realiza la operación
        public string CashierUserId { get; set; } = string.Empty;
    }
}
