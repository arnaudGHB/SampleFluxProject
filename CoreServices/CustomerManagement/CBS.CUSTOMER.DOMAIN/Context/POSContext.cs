

using CBS.CUSTOMER.DATA;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Entity.ChangePhoneNumber;
using CBS.CUSTOMER.DATA.Entity.CMoney;
using CBS.CUSTOMER.DATA.Entity.CMoneyP;
using CBS.CUSTOMER.DATA.Entity.Config;
using CBS.CUSTOMER.DATA.Entity.Customers;
using CBS.CUSTOMER.DATA.Entity.Document;
using CBS.CUSTOMER.DATA.Entity.Groups;
using Microsoft.EntityFrameworkCore;

namespace CBS.CUSTOMER.DOMAIN.Context
{
    public class POSContext : DbContext
    {
        public POSContext(DbContextOptions<POSContext> options) : base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<MembershipNextOfKing> MembershipNextOfKings { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<AccountSignature> CardSignatureSpecimens { get; set; }
        public DbSet<CustomerCategory> CustomerCategories { get; set; }
        public DbSet<GroupCustomer> GroupCustomers { get; set; }
         public DbSet<GroupType> GroupTypes { get; set; }
        public DbSet<GroupDocument> GroupDocuments { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<CustomerDocument> Documents { get; set; }
        public DbSet<CustomerAgeCategory> CustomerAgeCategores { get; set; }

        public DbSet<CMoneyMembersActivationAccount> CMoneyMembersActivationAccounts { get; set; }
        public DbSet<CMoneySubscriptionDetail> CMoneySubscriptionDetails { get; set; }
        public DbSet<OrganizationCustomer> OrganizationCustomers { get; set; }
        public DbSet<CustomerSmsConfigurations> CustomerSmsConfigurations { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<DocumentBaseUrl> DocumentBaseUrls { get; set; }
        public DbSet<EmployeeLeave> EmployeeLeaves { get; set; }
        public DbSet<EmployeeTraining> EmployeeTraining { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<PhoneNumberChangeHistory> PhoneNumberChangeHistories { get; set; }
        //
        public DbSet<UploadedCustomerWithError> UploadedCustomerWithErrors { get; set; }
        public DbSet<AndriodVersionConfiguration> AndriodVersionConfigurations { get; set; }
    
    }
}
