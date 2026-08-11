using ABP.Core.Application.Dtos.Common;
using ABP.Core.Application.Dtos.Loans;
using ABP.Core.Domain.Common.Enums;
using MediatR;


namespace ABP.Core.Application.Features.Loans.Queries
{
    public class GetLoansListQuery : IRequest<PagedResult<LoanDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public LoanStatus? Status { get; set; }
        // TODO: agregar filtro por cédula cuando Persona1 defina la interfaz de Identity
    }
}
