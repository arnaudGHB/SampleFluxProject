using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class TrialBalance6ColumnDto
    {
        public string EntityId { get; set; }
        public string EntityType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? BranchName { get; set; }
        public string? BranchLocation { get; set; }
        public string? BranchAddress { get; set; }
        public string Capital { get; set; }
        public string ImmatriculationNumber { get; set; }
        public string WebSite { get; set; }
        public string BranchTelephone { get; set; }
        public string HeadOfficeTelePhone { get; set; }
        public string BranchCode { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountNumberCU { get; set; }
        public string? AccountName { get; set; }
        public double BeginningDebitBalance { get; set; }
        public double BeginningCreditBalance { get; set; }
        public double DebitBalance { get; set; }
        public double CreditBalance { get; set; }  
        public double EndDebitBalance { get; set; }
        public double EndCreditBalance { get; set; }
        public string? TotalBeginningDebitBalance { get; set; }
        public string? TotalBeginningCreditBalance { get; set; }
        public string? TotalDebitBalance { get; set; }
        public string? TotalCreditBalance { get; set; }
        public string? TotalEndDebitBalance { get; set; }
        public string? TotalEndCreditBalance { get; set; }
        public string Cartegory { get; set; }

        public ReportHeader ConvertToHeaderReport()
        {
            return new ReportHeader
            {
                FromDate = FromDate,
                ToDate = ToDate,
                WebSite = WebSite,
                Address = Address,
                BranchTelephone = BranchTelephone,
                Location = Location,
                Capital = Capital,
                BranchAddress = BranchAddress,
                BranchCode = BranchCode,
                BranchLocation = BranchLocation,
                BranchName = BranchName,
                EntityId = EntityId,
                HeadOfficeTelePhone = HeadOfficeTelePhone,
                EntityType = EntityType,
                ImmatriculationNumber = ImmatriculationNumber,
                Name = Name
         

            };
        }

        public TrialBalance ConvertToReportedTrialBalance()
        {
            return new TrialBalance
            {
                AccountName = AccountName,
                AccountNumber = AccountNumber,
                DebitBalance = DebitBalance,
                CreditBalance = CreditBalance,
                EndingDebitBalance = EndDebitBalance,
                BeginningCreditBalance = BeginningCreditBalance,
                EndingCreditBalance = EndCreditBalance,
                BeginningDebitBalance = BeginningDebitBalance,
                Account1 = AccountNumber.Substring(0, 1),
                Account2 = AccountNumber.Substring(0, 2),
                Account3 = AccountNumber.Substring(0, 3),
                Account4 = AccountNumber.Substring(0, 4),
                Account5 = AccountNumber.Substring(0, 5)
            };
        }
     
    }





}
