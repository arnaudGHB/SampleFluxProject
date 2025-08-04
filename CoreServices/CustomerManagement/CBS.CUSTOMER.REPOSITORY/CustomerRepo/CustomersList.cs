using CBS.CUSTOMER.DATA.Dto.Customers;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.HELPER.Helper;
using Microsoft.EntityFrameworkCore;

namespace CBS.CUSTOMER.REPOSITORY.CustomerRepo
{


    public class CustomersList : List<GetAllCustomers>
    {
        public CustomersList()
        {
        }

        public int Skip { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public PaginationMetadata PaginationMetadata;

        public CustomersList(List<GetAllCustomers> items, int count, int skip, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            Skip = skip;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public async Task<CustomersList> Create(IQueryable<DATA.Entity.Customer> source, int skip, int pageSize)
        {
            var count = await GetCount(source);
            PaginationMetadata = new PaginationMetadata
            {
                PageSize = pageSize,
                Skip = skip,
                TotalCount = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
            };
            var dtoList = await GetDtos(source, skip, pageSize);
            var dtoPageList = new CustomersList(dtoList, count, skip, pageSize);
            return dtoPageList;
        }

        public async Task<int> GetCount(IQueryable<DATA.Entity.Customer> source)
        {
            return await source.AsNoTracking().CountAsync();
        }

        public async Task<List<GetAllCustomers>> GetDtos(IQueryable<DATA.Entity.Customer> source, int skip, int pageSize)
        {
            var entities = await source
                .Skip(skip)
                .Take(pageSize)
                .AsNoTracking()
                .Select(c => new GetAllCustomers
                {
                    CustomerId = c.CustomerId,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    IsMemberOfACompany = c.IsMemberOfACompany,
                    VillageOfOrigin = c.VillageOfOrigin,
                    IsBelongToGroup = c.IsBelongToGroup,
                    IsMemberOfAGroup = c.IsMemberOfAGroup,
                    LegalForm = c.LegalForm,
                    FormalOrInformalSector = c.FormalOrInformalSector,
                    BankingRelationship = c.BankingRelationship,
                    DateOfBirth = c.DateOfBirth,
                    PlaceOfBirth = c.PlaceOfBirth,
                    Occupation = c.Occupation,
                    Address = c.Address,
                    IDNumber = c.IDNumber,
                    IDNumberIssueDate = c.IDNumberIssueDate,
                    IDNumberIssueAt = c.IDNumberIssueAt,
                    MembershipApplicantDate = c.MembershipApplicantDate,
                    MembershipApplicantProposedByReferral1 = c.MembershipApplicantProposedByReferral1,
                    MembershipApplicantProposedByReferral2 = c.MembershipApplicantProposedByReferral2,
                    MembershipApprovalBy = c.MembershipApprovalBy,
                    MembershipApprovalStatus = c.MembershipApprovalStatus,
                    MembershipApprovedDate = c.MembershipApprovedDate,
                    MembershipApprovedSignatureUrl = c.MembershipApprovedSignatureUrl,
                    MembershipAllocatedNumber = c.MembershipAllocatedNumber,
                    POBox = c.POBox,
                    Fax = c.Fax,
                    Gender = c.Gender,
                    Email = c.Email,
                    Phone = c.Phone,
                    CountryId = c.CountryId,
                    RegionId = c.RegionId,
                    TownId = c.TownId,
                    PhotoUrl = c.PhotoUrl,
                    IsUseOnLineMobileBanking = c.IsUseOnLineMobileBanking,
                    MobileOrOnLineBankingLoginState = c.MobileOrOnLineBankingLoginState,
                    CustomerPackageId = c.CustomerPackageId,
                    CustomerCode = c.CustomerCode,
                    BankName = c.BankName,
                    DivisionId = c.DivisionId,
                    BranchId = c.BranchId,
                    EconomicActivitiesId = c.EconomicActivitiesId,
                    SignatureUrl = c.SignatureUrl,
                    BankId = c.BankId,
                    OrganizationId = c.OrganizationId,
                    Language = c.Language,
                    SubDivisionId = c.SubDivisionId,
                    TaxIdentificationNumber = c.TaxIdentificationNumber,
                    Active = c.Active,
                    SecretQuestion = c.SecretQuestion,
                    SecretAnswer = c.SecretAnswer,
                    EmployerName = c.EmployerName,
                    EmployerTelephone = c.EmployerTelephone,
                    EmployerAddress = c.EmployerAddress,
                    MaritalStatus = c.MaritalStatus,
                    SpouseName = c.SpouseName,
                    SpouseAddress = c.SpouseAddress,
                    NumberOfKids = c.NumberOfKids,
                    SpouseOccupation = c.SpouseOccupation,
                    SpouseContactNumber = c.SpouseContactNumber,
                    Income = c.Income,
                    CustomerCategoryId = c.CustomerCategoryId,
                    WorkingStatus = c.WorkingStatus,
                    ActiveStatus = c.ActiveStatus,
                    RegistrationNumber = c.RegistrationNumber,
                    CompanyCreationDate = c.CompanyCreationDate,
                    PlaceOfCreation = c.PlaceOfCreation,
                    ConditionForWithdrawal = c.ConditionForWithdrawal,
                    PaginationMetadata = PaginationMetadata,
                    AccountConfirmationNumber=c.AccountConfirmationNumber,
                    Matricule=c.Matricule
                })
                .ToListAsync();
            return entities;
        }

    }


}
