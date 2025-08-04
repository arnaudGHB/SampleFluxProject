using AutoMapper;
using CBS.Customer.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.Customer.MEDIATR;
using CBS.Organisation.MEDIATR;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Dto.PinValidation;
using CBS.CustomerSmsConfigurations.MEDIAT;
using CBS.CUSTOMER.DATA.Dto.Customers;
using CBS.Organization.MEDIATR;
using CBS.CUSTOMER.DATA;
using CBS.CUSTOMER.MEDIATR;
using CBS.CUSTOMER.DATA.Entity.Document;
using CBS.CUSTOMER.DATA.Dto.Groups;
using CBS.CUSTOMER.DATA.Entity.CMoneyP;
using CBS.CUSTOMER.DATA.Entity.CMoney;
using CBS.CustomerSmsConfigurations.MEDIAT.CMoney.MembersActivation;
using CBS.CUSTOMER.DATA.Dto.CMoney;
using CBS.CUSTOMER.MEDIATR.Customer.Command;
using CBS.CUSTOMER.DATA.Entity.Customers;
using CBS.CUSTOMER.MEDIATR.CMoney.Configuration.AndriodVersion.Command;

namespace CBS.CUSTOMER.API.Helpers.MappingProfile
{
    public class CustomerProfiles : Profile
    {
        public CustomerProfiles()
        {
            CreateMap<DATA.Entity.Customer, CreateCustomer>().ReverseMap();
            CreateMap<DATA.Entity.Customer, UpdateCustomer>().ReverseMap();
            CreateMap<DATA.Entity.Customer, GetCustomerSecret>().ReverseMap();
            CreateMap<DATA.Entity.Customer, GetCustomerDateOfBirthDto>().ReverseMap();
            CreateMap<DATA.Entity.Customer, UpdateCustomerDateBirthDto>().ReverseMap();
       
            CreateMap<AddCustomerCommand, DATA.Entity.Customer>();
        
            CreateMap<GetAllCustomers,DATA.Entity.Customer>().ReverseMap();
            CreateMap<CustomerDto,DATA.Entity.Customer>().ReverseMap();
            CreateMap<AddCustomerCommand, CreateCustomer>().ReverseMap();
            CreateMap<UpdateCustomerCommand,DATA.Entity.Customer>();

     

            CreateMap<PinValidationCommand,PinValidationResponse>().ReverseMap();
            CreateMap<ChangePinCommand,ChangePinResponse>().ReverseMap();


            CreateMap<Question, CreateQuestion>().ReverseMap();
            CreateMap<Question, GetQuestion>().ReverseMap();
            CreateMap<Question, AddQuestionCommand>().ReverseMap();


            CreateMap<AndriodVersionConfiguration, AndriodVersionConfigurationDto>().ReverseMap();
            CreateMap<AndriodVersionConfiguration, AddOrUpdateAndroidVersionCommand>().ReverseMap();

            

            CreateMap<DATA.Entity.CustomerCategory, CreateCustomerCategory>().ReverseMap();
            CreateMap<AddCustomerCategoryCommand, CustomerCategory>().ReverseMap();
            CreateMap<CustomerCategory, GetAllCustomerCategory>().ReverseMap();


            CreateMap<DATA.Entity.Customer,CreateMembershipCustomer>().ReverseMap();
            CreateMap<DATA.Entity.Customer,AddMembershipCustomerCommand>().ReverseMap();

            CreateMap<CreateCardSignatureSpecimen, DATA.Entity.AccountSignature>().ReverseMap();
            CreateMap<GetCardSignatureSpecimen, DATA.Entity.AccountSignature>().ReverseMap();
            CreateMap<DATA.Entity.AccountSignature, AddCardSignatureSpecimenCommand>().ReverseMap();

            CreateMap<MembershipNextOfKing, CreateMembershipNextOfKings>().ReverseMap();
            CreateMap<MembershipNextOfKing, GetMembershipNextOfKings>().ReverseMap();
            CreateMap<MembershipNextOfKing, AddMembershipNextOfKingsCommand>().ReverseMap();



            CreateMap<DATA.Entity.CustomerSmsConfigurations, CreateCustomerSmsConfigurations>().ReverseMap();
            CreateMap<AddCustomerSmsConfigurationsCommand, CreateCustomerSmsConfigurations>().ReverseMap();
            CreateMap<AddCustomerSmsConfigurationsCommand, DATA.Entity.CustomerSmsConfigurations>().ReverseMap();


         

            CreateMap<CustomerDocument, GetCustomerDocument>().ReverseMap();



            CreateMap<DATA.Entity.GroupCustomer, CreateGroupCustomer>().ReverseMap();
            CreateMap<AddGroupCustomerCommand, CreateGroupCustomer>().ReverseMap();
            CreateMap<AddGroupCustomerCommand, DATA.Entity.GroupCustomer>().ReverseMap();

            CreateMap<DATA.Entity.GroupType, CreateGroupTypeDto>().ReverseMap();
            CreateMap<AddGroupTypeCommand, CreateGroupTypeDto>().ReverseMap();
            CreateMap<AddGroupTypeCommand, DATA.Entity.GroupType>().ReverseMap();

            CreateMap<DATA.Entity.GroupCustomer, UpdateGroupCustomer>().ReverseMap();
            CreateMap<UpdateGroupCustomerCommand, UpdateGroupCustomer>().ReverseMap();
            CreateMap<UpdateGroupCustomerCommand, DATA.Entity.GroupCustomer>().ReverseMap();

            CreateMap<DATA.Entity.GroupType, UpdateGroupTypeDto>().ReverseMap();
            CreateMap<UpdateGroupTypeCommand, UpdateGroupTypeDto>().ReverseMap();
            CreateMap<UpdateGroupTypeCommand, DATA.Entity.GroupType>().ReverseMap();


            CreateMap<DATA.Entity.Group, CreateGroup>().ReverseMap();
            CreateMap<AddGroupCommand, CreateGroup>().ReverseMap();
            CreateMap<AddGroupCommand, DATA.Entity.Group>().ReverseMap();   
            
            
            CreateMap<DATA.Entity.Group, UpdateGroup>().ReverseMap();
            CreateMap<UpdateGroupCommand, UpdateGroup>().ReverseMap();
            CreateMap<UpdateGroupCommand, DATA.Entity.Group>().ReverseMap();



            CreateMap<DATA.Entity.Organization, CreateOrganization>().ReverseMap();
            CreateMap<AddOrganizationCommand, CreateOrganization>().ReverseMap();
            CreateMap<AddOrganizationCommand, DATA.Entity.Organization>().ReverseMap();
            
            
            CreateMap<DATA.Entity.Organization, UpdateOrganization>().ReverseMap();
            CreateMap<UpdateOrganizationCommand, UpdateOrganization>().ReverseMap();
            CreateMap<UpdateOrganizationCommand, DATA.Entity.Organization>().ReverseMap();

               CreateMap<DATA.Entity.OrganizationCustomer, CreateOrganizationCustomer>().ReverseMap();
            CreateMap<AddOrganizationCustomerCommand, CreateOrganizationCustomer>().ReverseMap();
            CreateMap<AddOrganizationCustomerCommand, DATA.Entity.OrganizationCustomer>().ReverseMap();  


               CreateMap<DATA.Entity.OrganizationCustomer, UpdateOrganizationCustomer>().ReverseMap();
            CreateMap<UpdateOrganizationCustomerCommand, UpdateOrganizationCustomer>().ReverseMap();
            CreateMap<UpdateOrganizationCustomerCommand, DATA.Entity.OrganizationCustomer>().ReverseMap();


            CreateMap<CMoneyMembersActivationAccount, AddCMoneyMemberActivationCommand>().ReverseMap();
            CreateMap<CMoneyMembersActivationAccount, UpdateCMoneyMemberActivationCommand>().ReverseMap();
            CreateMap<CMoneyMembersActivationAccount, CMoneyMembersActivationAccountDto>().ReverseMap();

            CreateMap<AddCustomerAgeCategoryCommand, CustomerAgeCategory>();
            CreateMap<UpdateCustomerAgeCategoryCommand, CustomerAgeCategory>();
            CreateMap<CustomerAgeCategory, CustomerAgeCategoryDto>().ReverseMap();

        }
    }
}
