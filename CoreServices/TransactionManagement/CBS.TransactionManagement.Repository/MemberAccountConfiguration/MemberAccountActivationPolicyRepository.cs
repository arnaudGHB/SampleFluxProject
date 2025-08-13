using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.MemberAccountConfiguration;
using CBS.TransactionManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.MemberAccountConfiguration
{
   
    public class MemberAccountActivationPolicyRepository : GenericRepository<MemberRegistrationFeePolicy, TransactionContext>, IMemberAccountActivationPolicyRepository
    {
        public MemberAccountActivationPolicyRepository(IUnitOfWork<TransactionContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
