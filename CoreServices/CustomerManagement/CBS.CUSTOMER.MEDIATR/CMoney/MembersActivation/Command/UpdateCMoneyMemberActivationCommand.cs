

using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Dto.CMoney;

namespace CBS.CustomerSmsConfigurations.MEDIAT.CMoney.MembersActivation
{
    /// <summary>
    /// Command for updating a C-MONEY member's activation details.
    /// </summary>
    public class UpdateCMoneyMemberActivationCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// The ID of the activation account to be updated.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The updated phone number for the member.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Indicates whether the account is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Indicates whether the account is subscribed.
        /// </summary>
        public bool IsSubcribed { get; set; }
    }


}
