using ABP.Core.Application.Dtos.Loans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ABP.Core.Application.Interfaces
{
    public interface ILoanService : IGenericService<LoanDto>
    {
        Task<LoanDto?> GetByLoanNumberAsync(string loanNumber);
        Task<List<LoanDto>> GetAllByClientIdAsync(string clientId);
        Task<bool> ExistsLoanNumberAsync(string loanNumber);
    }
}
