using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.MemberAccountConfiguration;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.Repository.MemberAccountConfiguration
{
    public interface IMemberAccountActivationRepository : IGenericRepository<MemberAccountActivation>
    {
        Task<MemberFeePolicyDto> GetMemberSubcription(CustomerDto customer);
    }
}
