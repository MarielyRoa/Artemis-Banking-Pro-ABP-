using System.Linq;
using ABP.Core.Application.ViewModels.Loans;
using ABP.Core.Domain.Interfaces;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ABP.Core.Domain.Entities;
using ABP.Core.Domain.Common.Enums;

namespace ABP.Core.Application.Features.Loans.Queries.GetAllLoans
{
    public class GetAllLoansQuery : IRequest<IEnumerable<LoanViewModel>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Status { get; set; }
        public string? Identification { get; set; }
    }

    public class GetAllLoansQueryHandler : IRequestHandler<GetAllLoansQuery, IEnumerable<LoanViewModel>>
    {
        private readonly IGenericRepository<Loan> _repository;
        private readonly IMapper _mapper;

        public GetAllLoansQueryHandler(IGenericRepository<Loan> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LoanViewModel>> Handle(GetAllLoansQuery query, CancellationToken cancellationToken)
        {
            var allEntities = _repository.GetAllQuery();
            var entities = allEntities.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize);
            return _mapper.Map<IEnumerable<LoanViewModel>>(entities);
        }
    }
}

