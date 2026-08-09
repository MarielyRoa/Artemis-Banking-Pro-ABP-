using System.ComponentModel.DataAnnotations;

namespace ABP.Core.Application.Dtos.Cashier
{
    public class CreditCardPaymentDto
    {
        [Required]
        [StringLength(16, MinimumLength = 13)]
        public string CreditCardNumber { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero.")]
        public decimal Amount { get; set; }

        // Opcional: identificar al cajero que realiza la operación
        public string CashierUserId { get; set; } = string.Empty;
    }
}
