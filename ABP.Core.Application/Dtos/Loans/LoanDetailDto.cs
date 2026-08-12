
namespace ABP.Core.Application.Dtos.Loans
{
    public class LoanDetailDto : LoanDto
    {
        public List<LoanInstallmentDto> Installments { get; set; } = new();
    }
}
