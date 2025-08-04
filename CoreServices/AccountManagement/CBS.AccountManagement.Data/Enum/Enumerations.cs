using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public enum TellerDailyProvisionType
    {
        CashReplenishment_By_Primary_teller,
        CashReplenishment_By_Manager,
        DailyProvision,
        Teller
    }



    public enum AccountType_Product
    {
        Loan_Product ,
        Saving_Product ,
            Teller
    }

    public enum OperationEventAttributeTypes
    {
        deposit,
        loan_approval,
        loan_disbursement,
        withdrawal,
        reversal,
        transfer,
        cashreplenishment,
        none,
        Clossing_Of_Account_Operations,
        opening,
        Teller_Operation_Account,
    }
    public enum TransactionCode
    {
        CINT,
        COUT,
        TRANS,
        LD,
        CRP ,///cashreplenishment
        None,
    }

    public enum TransactionStatus
    {
        POSTED,
        CANCEL,
       
    }
    public enum OperationAccounts
    {
        Interest_Account,
        Expense_Account,
        Saving_Account,
      
    }
}
