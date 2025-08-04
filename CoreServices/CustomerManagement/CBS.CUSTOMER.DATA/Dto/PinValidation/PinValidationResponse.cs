using CBS.CUSTOMER.DATA.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto.PinValidation
{
    public class PinValidationResponse
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
        public bool IsBlocked { get; set; }


    }
}
