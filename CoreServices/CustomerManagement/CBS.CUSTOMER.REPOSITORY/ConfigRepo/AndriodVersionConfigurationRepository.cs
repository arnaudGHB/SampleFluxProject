using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA.Entity.CMoney;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.REPOSITORY.ConfigRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.REPOSITORY
{

    public class AndriodVersionConfigurationRepository : GenericRepository<AndriodVersionConfiguration, POSContext>, IAndriodVersionConfigurationRepository
    {
        public AndriodVersionConfigurationRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
