using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class GetAllTellerAccountsByBankIdAndBranchId : IRequest<ServiceResponse<List<AccountDto>>>
    {
        public string BankId { get; set; }
        public string BranchId { get; set; }
    }
}
