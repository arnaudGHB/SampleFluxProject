

using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Entity.CMoney;
using CBS.CUSTOMER.DATA.Dto.CMoney;

namespace CBS.CustomerSmsConfigurations.MEDIAT.CMoney.MembersActivation
{
    /// <summary>
    /// Represents a command to add a new CustomerSmsConfigurations.
    /// </summary>
    public class AddCMoneyMemberActivationCommand : IRequest<ServiceResponse<CMoneyMembersActivationAccountDto>>
    {

        public string CustomerId { get; set; } // e.g., "0011234567"
        public string BranchCode { get; set; } // e.g., "001"
        public string PhoneNumber { get; set; }
        public string BranchId { get; set; }
        public string OTP { get; set; }
        public string Language { get; set; }

    }

}
