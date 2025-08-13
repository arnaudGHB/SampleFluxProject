using AutoMapper;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.FeeP;
using CBS.TransactionManagement.Data.Dto.SavingProductFeeP;
using CBS.TransactionManagement.Data.Entity.FeeP;
using CBS.TransactionManagement.Data.Entity.SavingProductFeeP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.FeeMediaR.FeeP.Commands;
using CBS.TransactionManagement.MediatR.FeePolicyP.Commands;
using CBS.TransactionManagement.MediatR.SavingProductFeeP.Commands;

namespace CBS.TransactionManagement.API
{
    public class SavingProductProfile : Profile
    {
        public SavingProductProfile()
        {
            CreateMap<SavingProduct, SavingProductDto>().ReverseMap();
            CreateMap<AddSavingProductCommand, SavingProduct>();
            CreateMap<UpdateSavingProductCommand, SavingProduct>();


            CreateMap<FeePolicy, FeePolicyDto>().ReverseMap();
            CreateMap<AddFeePolicyCommand, FeePolicy>();
            CreateMap<UpdateFeePolicyCommand, FeePolicy>();


            CreateMap<Fee, FeeDto>().ReverseMap();
            CreateMap<AddFeeCommand, Fee>();
            CreateMap<UpdateFeeCommand, Fee>();



            CreateMap<SavingProductFee, SavingProductFeeDto>().ReverseMap();
            CreateMap<AddSavingProductFeeCommand, SavingProductFee>();
            CreateMap<UpdateSavingProductFeeCommand, SavingProductFee>();
        }
    }
}
