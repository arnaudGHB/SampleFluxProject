using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.DATA.Entity.Customers;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;


namespace CBS.CUSTOMER.REPOSITORY.CustomerRepo
{
    public class CustomerAgeCategoryRepository : GenericRepository<CustomerAgeCategory, POSContext>, ICustomerAgeCategoryRepository
    {
        public CustomerAgeCategoryRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }

}
