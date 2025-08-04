using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    public class TrialBalance4ColumnDto 
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
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountName { get; set; }
        public double BeginningBalance { get; set; }
        public string? BeginningBookingDirection { get; set; } = "CR";
        public double DebitBalance { get; set; }
        public double CreditBalance { get; set; }
        public double EndingBalance { get; set; }
        public string? EndingBookingDirection { get; set; }
        public double totalBeginningBookingDirection { get; set; }
        public double totalBeginningBalance { get; set; }
        public double totalDebitBalance { get; set; }
        public double totalCreditBalance { get; set; }
        public double totalEndingBalance { get; set; }
        public double totalEndingBookingDirection { get; set; }
        public string Cartegory { get; set; }

        public TrialBalance4column ConvertTo4Column()
        {
            return new TrialBalance4column
            {
                AccountName = AccountName,
                AccountNumber = AccountNumber,
                CreditBalance =CreditBalance,
                DebitBalance = DebitBalance,
                BeginningBalance = BeginningBalance,
                EndingBalance = EndingBalance
            };
        }
        public TrialBalance4ColumnDto ConvertTo4ColumnFromTBEndingBalnce(TrialBalance4ColumnDto trial)
        {
            trial.EndingBalance = FormatValue(EndingBalance);// > 0) ? "C " + ((EndingBalance==null)?"0.0": (Convert.ToDecimal(EndingBalance)).ToString("N")) : (Convert.ToDecimal(EndingBalance) == 0) ? EndingBalance : "D " + (Math.Abs( Convert.ToDecimal(EndingBalance))).ToString();
            trial.totalBeginningBalance = FormatValueB( totalBeginningBalance);// > 0) ? "C "  +((totalBeginningBalance == null) ? "0.0" : (Convert.ToDecimal(totalBeginningBalance)).ToString("N")) : "D " + (Math.Abs(Convert.ToDecimal(totalBeginningBalance))).ToString("N"); 
            trial.totalEndingBalance = FormatValueB(totalEndingBalance); //? "C " + ((totalEndingBalance == null) ? "0.0" : (Convert.ToDecimal(totalEndingBalance)).ToString("N")) : "D " + (Math.Abs(Convert.ToDecimal(totalEndingBalance))).ToString("N"); 
            trial.totalDebitBalance = FormatValueB(totalDebitBalance);// > 0) ? "C " + ((totalDebitBalance == null) ? "0.0" : (Convert.ToDecimal(totalDebitBalance)).ToString("N")) : "D " + (Math.Abs(Convert.ToDecimal(totalDebitBalance))).ToString("N");
            trial.totalCreditBalance = FormatValueB(totalCreditBalance);// > 0) ? "C " + ((totalCreditBalance == null) ? "0.0" : (Convert.ToDecimal(totalCreditBalance)).ToString("N")) : "D " + (Math.Abs(Convert.ToDecimal(totalCreditBalance))).ToString("N");


            return trial;
        }
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
                //BranchCode = BranchCode,
                BranchLocation = BranchLocation,
                BranchName = BranchName,
                EntityId = EntityId,
                HeadOfficeTelePhone = HeadOfficeTelePhone,
                EntityType = EntityType,
                ImmatriculationNumber = ImmatriculationNumber,
                Name = Name


            };
        }
        private double FormatValue(double endingBalance1)
        {
            double? endingBalance = Convert.ToDouble(endingBalance1);
            double result = 0;
           

            if (endingBalance > 0)
            {
                if (endingBalance == null)
                {
                    result = 0;
                }
                else
                {
                    result =endingBalance.Value;
                }
            }
            else if (endingBalance == 0)
            {
                result = endingBalance.Value;
            }
            else
            {
                double absEndingBalance = Math.Abs(endingBalance.Value);
                result =  absEndingBalance;
            }

            return result;
        }

        private double FormatValueB(object endingBalance1)
        {
            double? endingBalance = Convert.ToDouble(endingBalance1);
            double result = 0;


            if (endingBalance > 0)
            {
                if (endingBalance == null)
                {
                    result = 0;
                }
                else
                {
                    result =  ( endingBalance.Value);
                }
            }
            else if (endingBalance == 0)
            {
                result = endingBalance.Value;
            }
            else
            {
                double absEndingBalance = Math.Abs(endingBalance.Value);
                result =  absEndingBalance;
            }

            return result;
        }
    }

}
