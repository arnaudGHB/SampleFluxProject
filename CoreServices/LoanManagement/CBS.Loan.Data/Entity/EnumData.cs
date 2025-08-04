using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Entity
{
    public class EnumDto
    {
        public List<StringValue> LoanInterestMethods { get; set; }
        public List<StringValue> LoanInterestPeriods { get; set; }
        public List<StringValue> LoanInterestTypes { get; set; }
        public List<StringValue> LoanDurationPeriods { get; set; }
        public List<StringValue> CalculateInterestOn { get; set; }
        public List<StringValue> RepaymentCycles { get; set; }
        public List<StringValue> LoanStatuses { get; set; }
        public List<StringValue> RefundOrders { get; set; }
        public List<StringValue> LoanDisburstmentTypes { get; set; }
        public List<StringValue> LoanTypes { get; set; }
        public List<StringValue> PaymentModes { get; set; }
        public List<StringValue> PenaltyTypes { get; set; }
        public List<StringValue> YesOrNo { get; set; }
        public List<StringValue> LoanCommiteeValidationStatuses { get; set; }
        public List<StringValue> AmortizationTypes { get; set; }
        public List<StringValue> DisbursmentStatuses { get; set; }
        public List<StringValue> LoanTerms { get; set; }
        public List<StringValue> LoanTargets { get; set; }
        public List<StringValue> LoanCategories { get; set; }
        public List<StringValue> LoanDeliquenciesStatus { get; set; }
        public List<StringValue> LoanApplicationTypes { get; set; }
        
        //LoanStatus
    }
    public class StringValue
    {
        public string text { get; set; }
        public string value { get; set; }

        public StringValue(string name, string value) { text = name; this.value = value; }
        public StringValue(string name) { text = name; this.value = name; }

    }

 
}
