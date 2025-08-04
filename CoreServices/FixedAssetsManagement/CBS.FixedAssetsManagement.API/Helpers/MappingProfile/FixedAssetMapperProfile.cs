using AutoMapper;
using CBS.FixedAssetsManagement.Data;
using CBS.FixedAssetsManagement.MediatR.Commands;

namespace CBS.FixedAssetsManagement.API
{
    public class AssetProfile : Profile
    {
        public AssetProfile()
        {
            CreateMap<AddAssetCommand, AssetDto>().ReverseMap();
            CreateMap<AddAssetCommand, Asset>();
            CreateMap<UpdateAssetCommand, Asset>();
        }
    }

    public class AssetTypeProfile : Profile
    {
        public AssetTypeProfile()
        {
            CreateMap<AddAssetTypeCommand, AssetTypeDto>().ReverseMap();
            CreateMap<AddAssetTypeCommand, AssetType>();
            CreateMap<UpdateAssetTypeCommand, AssetType>();
        }
    }
    public class AssetDisposalProfile : Profile
    {
        public AssetDisposalProfile()
        {
            CreateMap<AddAssetDisposalCommand, AssetDisposalDto>().ReverseMap();
            CreateMap<AddAssetDisposalCommand, AssetDisposal>();
            CreateMap<UpdateAssetDisposalCommand, AssetDisposal>();
        }
    }
    public class AssetTransferProfile : Profile
    {
        public AssetTransferProfile()
        {
            CreateMap<AddAssetTransferCommand, AssetTransferDto>().ReverseMap();
            CreateMap<AddAssetTransferCommand, AssetTransfer>();
            CreateMap<UpdateAssetTransferCommand, AssetTransfer>();
        }
    }// 

    public class AssetRevaluationProfile : Profile
    {
        public AssetRevaluationProfile()
        {
            CreateMap<AddAssetRevaluationCommand, AssetRevaluationDto>().ReverseMap();
            CreateMap<AddAssetRevaluationCommand, AssetRevaluation>();
            CreateMap<UpdateAssetRevaluationCommand, AssetRevaluation>();
        }
    }
    //services.AddScoped<IDepreciationEntryRepository, DepreciationEntryRepository>();
    //        services.AddScoped<IDepreciationMethodRepository, DepreciationMethodRepository>();
    //        services.AddScoped<IDepreciationMethodRepository, DepreciationMethodRepository>();
    //        services.AddScoped<IMaintenanceLogRepository, MaintenanceLogRepository>();

    public class DepreciationEntryProfile : Profile
    {
        public DepreciationEntryProfile()
        {
            CreateMap<AddDepreciationEntryCommand, DepreciationEntryDto>().ReverseMap();
            CreateMap<AddDepreciationEntryCommand, DepreciationEntry>();
            CreateMap<UpdateDepreciationEntryCommand, DepreciationEntry>();
        }
    }
    public class DepreciationMethodProfile : Profile
    {
        public DepreciationMethodProfile()
        {
            CreateMap<AddDepreciationMethodCommand, DepreciationMethodDto>().ReverseMap();
            CreateMap<AddDepreciationMethodCommand, DepreciationMethod>();
            CreateMap<UpdateDepreciationMethodCommand, DepreciationMethod>();
        }

    }
    public class LocationProfile : Profile
    {
        public LocationProfile()
        {
            CreateMap<AddLocationCommand, LocationDto>().ReverseMap();
            CreateMap<AddLocationCommand, Location>();
            CreateMap<UpdateLocationCommand, Location>();
        }
    }
    public class MaintenanceLogProfile : Profile
    {
        public MaintenanceLogProfile()
        {
            CreateMap<AddMaintenanceLogCommand, MaintenanceLogDto>().ReverseMap();
            CreateMap<AddMaintenanceLogCommand, MaintenanceLog>();
            CreateMap<UpdateMaintenanceLogCommand, MaintenanceLog>();
        }
    }

}