
using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Resources;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.REPOSITORY
{
    public interface ICustomerRepository : IGenericRepository<DATA.Entity.Customer>
    {
        Task<CustomersList> GetCustomersAsync(CustomerResource customerResource);
        Task<CustomersList> GetCustomersAsyncByBranch(CustomerResource customerResource);
        Task<List<CustomerListingDto>> GetCustomerListingDto(string queryParameter, DateTime dateFrom, DateTime dateTo, string branchId, string legalForm, string membershipStatusType);


        Task<List<CustomerListingDto>> GetCustomerListingDto(
       string queryParameter, DateTime dateFrom, DateTime dateTo,
       string branchId, string legalFormStatus, string membersStatusType,
       int pageNumber = 1, int pageSize = 3500);


    }


}
