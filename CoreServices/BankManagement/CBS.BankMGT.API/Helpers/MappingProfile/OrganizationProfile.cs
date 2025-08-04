using AutoMapper;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.API
{
    public class OrganizationProfile: Profile
    {
        public OrganizationProfile()
        {
            CreateMap<Organization, OrganizationDto>().ReverseMap();
            CreateMap<AddOrganizationCommand, Organization>();
            CreateMap<UpdateOrganizationCommand, Organization>();
        }
    }
}
