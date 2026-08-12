using ABP.Core.Application.Dtos.SavingAccounts;
using ABP.Core.Domain.Entities;
using AutoMapper;


namespace ABP.Core.Application.Mappings.EntitiesAndDtos
{
    public class SavingAccountDetailMappingProfile : Profile
    {
        public SavingAccountDetailMappingProfile()
        {
            CreateMap<SavingAccount, SavingAccountDetailDto>()
                .ForMember(d => d.Transactions, opt => opt.MapFrom(s => s.Transactions));
        }
    }
}
