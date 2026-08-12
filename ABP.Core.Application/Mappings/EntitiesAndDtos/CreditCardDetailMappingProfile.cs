using ABP.Core.Application.Dtos.CreditCards;
using ABP.Core.Domain.Entities;
using AutoMapper;


namespace ABP.Core.Application.Mappings.EntitiesAndDtos
{
    public class CreditCardDetailMappingProfile : Profile
    {
        public CreditCardDetailMappingProfile()
        {
            CreateMap<CreditCard, CreditCardDetailDto>()
                .ForMember(d => d.Consumptions, opt => opt.MapFrom(s => s.CardTransactions));
        }
    }
}
