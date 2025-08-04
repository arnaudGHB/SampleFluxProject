 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
   
    public class TellerDailyProvision: BaseEntity
    {

        public TellerDailyProvision()
        {
                
        }
        public TellerDailyProvision(string eventCode, decimal amount, decimal surPlusAmount)
        {
             
            this.OpeningOfDayAmount = amount;
            this.SurPlusBalance = surPlusAmount;
            this.StateOfDay = GetStateOfDay(eventCode);
            this.OpeningState = GetOpeningStateOfDay(eventCode);
            this.ClosingState = GetOpeningStateOfDay(eventCode);
            this.TellerDailyProvisionType =  Data.TellerDailyProvisionType.DailyProvision.ToString().ToUpper();
            this.AdditionalProvision = 0;
            if (this.StateOfDay.Equals("OPENED"))
            {
                this.OpeningOfDayAmount = amount;
                this.SurPlusBalance = surPlusAmount;
            }
            else
            {
                this.CloseOfDayAmount = amount;
                this.SurPlusBalance = surPlusAmount;
            }
            
        }

    

        private static string? GetOpeningStateOfDay(string eventCode)
        {
            if (eventCode.Equals(OpeningAndClosingOfDayEnum.OOD))
            {
                return "Normal Opening Of Day";
            }
            else if (eventCode.Equals(OpeningAndClosingOfDayEnum.NEGATIVE_OOD))
            {
                return "Negative Opening Of Day";
            }
            else if (eventCode.Equals(OpeningAndClosingOfDayEnum.POSITIVE_OOD))
            {
                return "Positive Opening Of Day";
            }
            else if (eventCode.Equals(OpeningAndClosingOfDayEnum.COD))
            {
                return "Normal Closing Of Day";
            }
            else if (eventCode.Equals(OpeningAndClosingOfDayEnum.NEGATIVE_COD))
            {
                return "Negative Closing Of Day";
            }
            else if (eventCode.Equals(OpeningAndClosingOfDayEnum.POSITIVE_COD))
            {
                return "Positive Closing Of Day";
            }
            else
            {
                return "CashReplenishment Event Or Ambigous state";
            }
        }

        private static string? GetStateOfDay(string eventCode)
        {
            if (eventCode.Contains(OpeningAndClosingOfDayEnum.OOD))
            {
                return "OPENED";
            }
            else
            {
                return "PARTIALLY CLOSED";
            }
        }

      

        public  TellerDailyProvision UpdateTellerProvision(string eventCode, decimal amount, decimal amountDifference)
        {
         
            this.SurPlusBalance = amountDifference;
            this.StateOfDay = GetStateOfDay(eventCode);
            this.OpeningState = GetOpeningStateOfDay(eventCode);
            this.ClosingState = GetOpeningStateOfDay(eventCode);
          
                 

            return this;
        }

        public string ?OpeningState { get; set; } = "";

        public string? ClosingState { get; set; } = "";

        public decimal OpeningOfDayAmount { get; set; } = 0;

        public decimal CloseOfDayAmount { get; set; } = 0;

        public decimal AdditionalProvision { get; set; } = 0;

        public decimal SurPlusBalance { get; set; } = 0;

        public string TellerDailyProvisionType { get; set; } //CashReplenishment or DailyProvision

        public decimal CashAtHand { get; set; } = 0;

        public string? StateOfDay { get; set; } = "";

        public string? Comment { get; set; } = "";
        public string Id { get; set; }
    }
}
