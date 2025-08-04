using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto.CMoney
{
    /// <summary>
    /// DTO for the login response.
    /// </summary>
    public class LoginResponseDto
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Occupation { get; set; }
        public string Address { get; set; }
        public string IdNumber { get; set; }
        public string IdNumberIssueDate { get; set; }
        public string IdNumberIssueAt { get; set; }
        public string MembershipApprovalStatus { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string CustomerCode { get; set; }
        public string BankName { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsUseOnLineMobileBanking { get; set; }
        public string Language { get; set; }
        public bool Active { get; set; }
        public string Telephone { get; set; }
       public string NotificationToken { get; set; }
        public bool IsBlocked { get; set; }
        public string LoginId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string Message { get; set; }
        public bool ChangePinIsRequired { get; set; }
        public string SecretAnswer { get; set; }
        public string SecretQuestion { get; set; }
        public string ApkUrl { get; set; }
    }

}
