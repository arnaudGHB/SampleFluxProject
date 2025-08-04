// Ignore Spelling: Dto

using CBS.CUSTOMER.HELPER.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto.Global
{
    public class SubscriptionAggregatesDto
    {
        public List<DropDownListDto>? LegalForms { get; set; }
        public List<DropDownListDto>? ProfileType { get; set; }
        public List<DropDownListDto>? WorkingStatuses { get; set; }
        public List<DropDownListDto>? ActiveStatuses { get; set; }
        public List<DropDownListDto>? MaritalStatuses { get; set; }
        public List<DropDownListDto>? BankingRelationships { get; set; }
        public List<DropDownListDto>? FormalOrInformalSectors { get; set; }
        public List<DropDownListDto>? Relationships { get; set; }
        public List<DropDownListDto>? Genders { get; set; }
        public List<DropDownListDto>? DocumentTypes { get; set; }
        public List<DropDownListDto>? CustomerCategories { get; set; }

        public List<DropDownListDto>? MembershipApprovalStatuses { get; set; }

    }
}
