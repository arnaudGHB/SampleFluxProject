

using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA.Entity.ChangePhoneNumber;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.REPOSITORY.ChangePhoneNumber;
using CBS.CUSTOMER.REPOSITORY.CMoney.MembersActivation;
using CBS.CUSTOMER.REPOSITORY.CMoney.SubcriptionDetail;
using CBS.CUSTOMER.REPOSITORY.ConfigRepo;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;
using CBS.CUSTOMER.REPOSITORY.DocumentRepo;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;
using CBS.CUSTOMER.REPOSITORY.OrganisationRepo;

namespace CBS.CUSTOMER.API.Helpers.DependencyResolver
{
    public static class DependencyInjectionExtension
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICardSignatureSpecimenRepository, CardSignatureSpecimenRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IGroupCustomerRepository, GroupCustomerRepository>();
            services.AddScoped<IGroupTypeRepository, GroupTypeRepository>();
            services.AddScoped<IGroupDocumentRepository, GroupDocumentRepository>();
            services.AddScoped<IMembershipNextOfKingRepository, MembershipNextOfKingRepository>();
            services.AddScoped<ICustomerCategoryRepository, CustomerCategoryRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IOrganizationCustomerRepository, OrganizationCustomerRepository>();
            services.AddScoped<ICustomerSmsConfigurationRepository, CustomerSmsConfigurationRepository>();
            services.AddScoped<IPropertyMappingService, PropertyMappingService>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmployeeTrainingRepository, EmployeeTrainingRepository>();
            services.AddScoped<IEmployeeLeaveRepository, EmployeeLeaveRepository>();
            services.AddScoped<ILeaveTypeRepository, LeaveTypeRepository>();
            services.AddScoped<IJobTitleRepository, JobTitleRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IDocumentBaseUrlRepository, DocumentBaseUrlRepository>();
            services.AddScoped<IUploadedCustomerWithErrorRepository, UploadedCustomerWithErrorRepository>();
            services.AddScoped<ICMoneyMembersActivationAccountRepository, CMoneyMembersActivationAccountRepository>();
            services.AddScoped<ICMoneySubscriptionDetailRepository, CMoneySubscriptionDetailRepository>();
            services.AddScoped<SmsHelper, SmsHelper>();
            services.AddScoped<IPhoneNumberChangeHistoryRepository, PhoneNumberChangeHistoryRepository>();
            services.AddScoped<ICustomerAgeCategoryRepository, CustomerAgeCategoryRepository>();
            services.AddScoped<IAndriodVersionConfigurationRepository, AndriodVersionConfigurationRepository>();
            


        }
    }
}
