using ABP.Core.Application.Dtos.Loans;
using ABP.Core.Domain.Entities;
using AutoMapper;


namespace ABP.Core.Application.Mappings.EntitiesAndDtos
{
    public class LoanDetailMappingProfile : Profile
    {
        public LoanDetailMappingProfile()
        {
            CreateMap<Loan, LoanDetailDto>()
                .ForMember(d => d.Installments, opt => opt.MapFrom(s => s.LoanInstallments));
        }
    }
}
