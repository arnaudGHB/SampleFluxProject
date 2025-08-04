using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Resources;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.REPOSITORY
{

    public class CustomerRepository : GenericRepository<Customer, POSContext>, ICustomerRepository
    {
        private readonly IPropertyMappingService _propertyMappingService;

        public CustomerRepository(IUnitOfWork<POSContext> unitOfWork, IPropertyMappingService propertyMappingService = null) : base(unitOfWork)
        {
            _propertyMappingService = propertyMappingService;
        }

        public async Task<CustomersList> GetCustomersAsync(CustomerResource customerResource)
        {
            // Define the base query for customers
            IQueryable<DATA.Entity.Customer> query = All.AsNoTracking().Where(x => x.IsDeleted == false);


            // Apply sorting based on the provided order by property
            query = query.ApplySort(customerResource.OrderBy, _propertyMappingService.GetPropertyMapping<CustomerDto, Customer>()).OrderBy(x => x.FirstName);

            // Filter customers based on SearchQuery if provided
            if (!string.IsNullOrWhiteSpace(customerResource.SearchQuery) && customerResource.SearchQuery.ToLower() != "all")
            {
                string searchQueryLower = customerResource.SearchQuery.ToLower();
                query = query.Where(c =>
                    (c.FirstName != null && c.FirstName.ToLower().Contains(searchQueryLower)) ||
                    (c.LastName != null && c.LastName.ToLower().Contains(searchQueryLower)) ||
                    (c.Phone != null && c.Phone.ToLower().Contains(searchQueryLower)) ||
                    (c.MembershipApprovalStatus != null && c.MembershipApprovalStatus.ToLower().Contains(searchQueryLower)) ||
                    (c.IDNumber != null && c.IDNumber.ToLower().Contains(searchQueryLower)) ||
                    (c.CustomerId != null && c.CustomerId.ToLower().Contains(searchQueryLower)) ||
                    (c.Matricule != null && c.Matricule.ToLower().Contains(searchQueryLower)) ||
                    (c.AccountConfirmationNumber != null && c.AccountConfirmationNumber.ToLower().Contains(searchQueryLower)) ||
                    (!string.IsNullOrEmpty(c.FirstName) && !string.IsNullOrEmpty(c.LastName) && (c.FirstName.ToLower() + " " + c.LastName.ToLower()).Contains(searchQueryLower)));
            }


            // Create and return the paginated list of customers
            var customersList = new CustomersList();
            return await customersList.Create(query, customerResource.Skip, customerResource.PageSize);
        }

        public async Task<CustomersList> GetCustomersAsyncByBranch(CustomerResource customerResource)
        {
            // Define the base query for customers
            IQueryable<DATA.Entity.Customer> query = FindBy(x => x.BranchId == customerResource.BranchId && x.IsDeleted == false).AsNoTracking();

            // Apply sorting based on the provided order by property

            query = query.ApplySort(customerResource.OrderBy, _propertyMappingService.GetPropertyMapping<CustomerDto, Customer>()).OrderBy(x => x.FirstName);

            // Filter customers based on SearchQuery if provided
            if (!string.IsNullOrWhiteSpace(customerResource.SearchQuery) && customerResource.SearchQuery.ToLower() != "all")
            {
                string searchQueryLower = customerResource.SearchQuery.ToLower();
                query = query.Where(c =>
                    (c.FirstName != null && c.FirstName.ToLower().Contains(searchQueryLower)) ||
                    (c.LastName != null && c.LastName.ToLower().Contains(searchQueryLower)) ||
                    (c.Phone != null && c.Phone.ToLower().Contains(searchQueryLower)) ||
                    (c.MembershipApprovalStatus != null && c.MembershipApprovalStatus.ToLower().Contains(searchQueryLower)) ||
                    (c.IDNumber != null && c.IDNumber.ToLower().Contains(searchQueryLower)) ||
                    (c.CustomerId != null && c.CustomerId.ToLower().Contains(searchQueryLower)) ||
                    (c.Matricule != null && c.Matricule.ToLower().Contains(searchQueryLower)) ||
                    (c.AccountConfirmationNumber != null && c.AccountConfirmationNumber.ToLower().Contains(searchQueryLower)) ||

                    (!string.IsNullOrEmpty(c.FirstName) && !string.IsNullOrEmpty(c.LastName) && (c.FirstName.ToLower() + " " + c.LastName.ToLower()).Contains(searchQueryLower)));
            }


            // Create and return the paginated list of customers
            var customersList = new CustomersList();
            return await customersList.Create(query, customerResource.Skip, customerResource.PageSize);
        }
        public async Task<List<CustomerListingDto>> GetCustomerListingDto(string queryParameter, DateTime dateFrom, DateTime dateTo, string branchId, string legalFormStatus, string membersStatusType)
        {
            IQueryable<Customer> query = FindBy(x => !x.IsDeleted).AsNoTracking();

            // Apply branch filter only if provided
            if (!string.IsNullOrEmpty(branchId) && !branchId.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.BranchId == branchId);
            }

            // Apply date filtering only when "bydate" is specified
            if (queryParameter.Equals("bydate", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.CreatedDate.Date >= dateFrom.Date && x.CreatedDate.Date <= dateTo.Date);
            }

            // Apply legal form filtering unless "Both" is selected
            if (!string.IsNullOrEmpty(legalFormStatus) && !legalFormStatus.Equals("Both", StringComparison.OrdinalIgnoreCase))
            {
                if (Enum.TryParse(legalFormStatus, out LegalForm legalFormEnum))
                {
                    string legalFormString = legalFormEnum.ToString();
                    query = query.Where(x => x.LegalForm.ToLower() == legalFormString.ToLower());
                }
            }
            // Apply matricule filtering if "bymatricule" is specified
            if (queryParameter.Equals("bymatricule", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(queryParameter))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.Matricule));
            }

            // Apply membership status filtering unless "Both" is selected
            if (!string.IsNullOrEmpty(membersStatusType) && !membersStatusType.Equals("Both", StringComparison.OrdinalIgnoreCase))
            {
                if (Enum.TryParse(membersStatusType, out MembershipApprovalStatus membershipStatusEnum))
                {
                    string membershipStatusString = membershipStatusEnum.ToString();
                    query = query.Where(x => x.MembershipApprovalStatus.ToLower() == membershipStatusString.ToLower());
                }
            }

            // Sort results by FirstName
            query = query.OrderBy(x => x.FirstName);

            // Project results to DTO with optimized field selection
            var customerList = await query
                .Select(c => new CustomerListingDto
                {
                    CustomerId = c.CustomerId,
                    Name = c.FirstName + " " + c.LastName,
                    BankingRelationship = c.BankingRelationship,
                    RegistrationDate = c.CreatedDate,
                    IDNumber = c.IDNumber,
                    IDNumberIssueDate = c.IDNumberIssueDate,
                    MembershipApprovalStatus = c.MembershipApprovalStatus,
                    Address = c.Address,
                    BranchId = c.BranchId,
                    PhoneNumber = c.Phone,
                    AccountConfirmationNumber = c.AccountConfirmationNumber,
                    Matricule = c.Matricule, Phone=c.Phone
                })
                .ToListAsync();

            return customerList;
        }


        public async Task<List<CustomerListingDto>> GetCustomerListingDto(
    string queryParameter, DateTime dateFrom, DateTime dateTo,
    string branchId, string legalFormStatus, string membersStatusType,
    int pageNumber = 1, int pageSize = 3500)
        {
            IQueryable<Customer> query = FindBy(x => !x.IsDeleted).AsNoTracking();

            // Apply branch filter only if provided
            if (!string.IsNullOrEmpty(branchId) && !branchId.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.BranchId == branchId);
            }

            // Apply date filtering only when "bydate" is specified
            if (queryParameter.Equals("bydate", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.CreatedDate >= dateFrom && x.CreatedDate <= dateTo);
            }

            // Apply matricule filtering if "bymatricule" is specified
            if (queryParameter.Equals("bymatricule", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.Matricule));
            }

            // Apply legal form filtering unless "Both" is selected
            if (!string.IsNullOrEmpty(legalFormStatus) && !legalFormStatus.Equals("Both", StringComparison.OrdinalIgnoreCase))
            {
                if (Enum.TryParse(legalFormStatus, out LegalForm legalFormEnum))
                {
                    query = query.Where(x => x.LegalForm == legalFormEnum.ToString());
                }
            }

            // Apply membership status filtering unless "Both" is selected
            if (!string.IsNullOrEmpty(membersStatusType) && !membersStatusType.Equals("Both", StringComparison.OrdinalIgnoreCase))
            {
                if (Enum.TryParse(membersStatusType, out MembershipApprovalStatus membershipStatusEnum))
                {
                    query = query.Where(x => x.MembershipApprovalStatus == membershipStatusEnum.ToString());
                }
            }

            // Sort results by FirstName before applying pagination
            query = query.OrderBy(x => x.FirstName)
                         .Skip((pageNumber - 1) * pageSize) // Implement pagination
                         .Take(pageSize);

            // Project results to DTO **after filtering and sorting**
            var customerList = await query
                .Select(c => new CustomerListingDto
                {
                    CustomerId = c.CustomerId,
                    Name = c.FirstName + " " + c.LastName,
                    BankingRelationship = c.BankingRelationship,
                    RegistrationDate = c.CreatedDate,
                    IDNumber = c.IDNumber,
                    IDNumberIssueDate = c.IDNumberIssueDate,
                    MembershipApprovalStatus = c.MembershipApprovalStatus,
                    Address = c.Address,
                    BranchId = c.BranchId,
                    PhoneNumber = c.Phone,
                    AccountConfirmationNumber = c.AccountConfirmationNumber,
                    Matricule = c.Matricule,
                    Phone=c.Phone
                })
                .ToListAsync();

            return customerList;
        }


    }
}
