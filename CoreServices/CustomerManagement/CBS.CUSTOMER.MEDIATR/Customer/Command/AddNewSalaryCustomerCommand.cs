
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using CBS.CUSTOMER.DATA.Dto;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR
{
    /// <summary>
    /// Represents a command to add a new salary customer.
    /// </summary>
    public class AddNewSalaryCustomerCommand : IRequest<ServiceResponse<CreateCustomer>>
    {

        public string? CustomerId { get; set; }
        public string? MemberName { get; set; }
        public string? Matricule { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }
        public string? BranchId { get; set; }
        public string? BankId { get; set; }
        public string? BankCode { get; set; }
        public bool IsFileData { get; set; }
        public string? CustomerType { get; set; }
    }

}
