using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data
{
   
    public class AccountRubrique
    {
        /// <summary>
        /// Deposit, Withdrawal, Transfer, Management fee, Opening Balance, Closing Balance, 
        /// </summary>
        public string operationEventAttributeName { get; set; }// Interest, Commision, Tax
        public bool isDebit { get; set; }
        /// <summary>
        /// Chart of account selection
        /// </summary>
        public string chartOfAccountId { get; set; }
    }
    public class AccountTRubriqueResponseObject
    {
        /// <summary>
        /// Product ID
        /// </summary>
        public string productAccountingBookId { get; set; }
        /// <summary>
        /// Interest, Commision, Tax, Fee, Other Fee etc
        /// </summary>
        public string operationType { get; set; }
        public List<AccountRubrique> accountRubriques { get; set; }

    }
}
