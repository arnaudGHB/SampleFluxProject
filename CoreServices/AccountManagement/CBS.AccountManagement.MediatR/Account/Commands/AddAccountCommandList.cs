using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using MediatR;
using System.Collections.Generic;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new Account.
    /// </summary>
    public class AddAccountCommandList : IRequest<ServiceResponse<List<AccountDto>>>
    {
        public List<AccountCommand> AccountCommands { get; set; }


        public static List<AccountCommand>  CreateListOfAccountwithAccountType(List<Data.AccountTypeDetail> AccountTypeDetails, string AccontTypeId, string ProductName, UserInfoToken userInfoToken)
        {
            List < AccountCommand > accountCommandList = new List < AccountCommand >();
            foreach (var item in AccountTypeDetails)
            {
                accountCommandList.Add(new Commands.AccountCommand {
                    AccountTypeId = AccontTypeId,
                    AccountOwnerId = userInfoToken.BranchId,
                     BranchId = userInfoToken.BranchId,
                    BankId = userInfoToken.BankId,
                    AccountName = userInfoToken.BranchCode + " "+ ProductName + " "+ item.Name,
                    AccountNumber ="",
                    ChartOfAccountManagementPositionId = item.ChartOfAccountId,

                });
            }
           return accountCommandList;
        }
    }


    public class AccountCommand : AddAccountCommand, IRequest<ServiceResponse<bool>>
    {
     

        public string BranchId { get; set; }
        public string BankId { get; set; }
    }


}