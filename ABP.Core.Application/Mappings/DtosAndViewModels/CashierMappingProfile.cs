using AutoMapper;
using ABP.Core.Application.Dtos;
using ABP.Core.Domain.Entities;

namespace ABP.Core.Application.Mappings.DtosAndViewModels
{
    public class CashierMappingProfile : Profile
    {
        public CashierMappingProfile()
        {
            CreateMap<Transaction, TransactionDto>();
            CreateMap<DepositDto, Transaction>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => TransactionType.Credit))
                .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => "DEPÓSITO"))
                .ForMember(dest => dest.Beneficiary, opt => opt.MapFrom(src => src.DestinationAccountNumber))
                .ForMember(dest => dest.SavingAccountId, opt => opt.Ignore()) // set in service
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TransactionStatus.Completed));
            CreateMap<WithdrawalDto, Transaction>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => TransactionType.Debit))
                .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => "RETIRO"))
                .ForMember(dest => dest.Beneficiary, opt => opt.MapFrom(src => src.SourceAccountNumber))
                .ForMember(dest => dest.SavingAccountId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TransactionStatus.Completed));
            // Otros mapeos pueden agregarse aquí según se necesiten.
            // Mapeos para los DTOs de pago y transferencia del cajero
            CreateMap<CreditCardPaymentDto, Transaction>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => TransactionType.Debit))
                .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => "PAGO TARJETA"))
                .ForMember(dest => dest.Beneficiary, opt => opt.MapFrom(src => src.CreditCardNumber))
                .ForMember(dest => dest.SavingAccountId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TransactionStatus.Completed));

            CreateMap<LoanPaymentDto, Transaction>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => TransactionType.Debit))
                .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => "PAGO PRÉSTAMO"))
                .ForMember(dest => dest.Beneficiary, opt => opt.MapFrom(src => src.LoanNumber))
                .ForMember(dest => dest.SavingAccountId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TransactionStatus.Completed));

            CreateMap<ThirdPartyTransferDto, Transaction>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => TransactionType.Debit))
                .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => $"TRANSFERENCIA TERCEROS {src.DestinationAccountNumber}"))
                .ForMember(dest => dest.Beneficiary, opt => opt.MapFrom(src => src.DestinationAccountNumber))
                .ForMember(dest => dest.SavingAccountId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TransactionStatus.Completed));
        }
    }
}
