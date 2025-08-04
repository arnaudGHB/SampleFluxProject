using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new Account.
    /// </summary>
    public class AddLoanAccountCommand : IRequest<ServiceResponse<bool>>
    {
        public string CustomerId { get; set; }
        public string? FileUploadId { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public bool IsBGS { get; set; }
        public bool IsForSalaryTreatement { get; set; }
        public UserInfoToken? UserInfoToken { get; set; }
        public AddLoanAccountCommand()
        {
            UserInfoToken=new UserInfoToken();
        }
    }

}
