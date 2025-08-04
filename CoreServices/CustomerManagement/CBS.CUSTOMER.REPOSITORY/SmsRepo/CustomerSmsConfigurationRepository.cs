using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.REPOSITORY
{

    public class CustomerSmsConfigurationRepository : GenericRepository<DATA.Entity.CustomerSmsConfigurations, POSContext>, ICustomerSmsConfigurationRepository
    {
        public CustomerSmsConfigurationRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
