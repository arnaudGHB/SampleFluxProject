using CBS.TransactionManagement.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Dto
{
    public class CurrencyNotesRequest
    {
        public int Note10000 { get; set; }
        public int Note5000 { get; set; }
        public int Note2000 { get; set; }
        public int Note1000 { get; set; }
        public int Note500 { get; set; }
        public int Coin500 { get; set; }
        public int Coin100 { get; set; }
        public int Coin50 { get; set; }
        public int Coin25 { get; set; }
        public int Coin10 { get; set; }
        public int Coin5 { get; set; }
        public int Coin1 { get; set; }
    }

    public class CashChangeDataCarrier
    {
        public CurrencyNotesRequest denominationsGiven { get; set; }
        public CurrencyNotesRequest denominationsReceived { get; set; }
        public string changeReason { get; set; }
        public string tellerId { get; set; }
        public DateTime accountingDate { get; set; }
        public string reference { get; set; }
        public string OpenningOfDayReference { get; set; }
        public string SystemName { get; set; }
    }
   
}
