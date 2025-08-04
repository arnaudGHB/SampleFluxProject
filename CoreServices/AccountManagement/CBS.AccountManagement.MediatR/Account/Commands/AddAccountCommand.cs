using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using MediatR;
using Microsoft.Extensions.ObjectPool;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new Account.
    /// </summary>
    public class AddAccountCommand : IRequest<ServiceResponse<bool>>
    {
        //public string BookingDirection { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountTypeId { get; set; } = "";
        public string AccountOwnerId { get; set; }
        public string ChartOfAccountManagementPositionId { get; set; }
        public string AccountNumberManagementPosition { get; set; }
        public string OwnerBranchCode { get; set; }
        public string LiaisonBranchCode { get; set; }
        public string AccountNumberNetwok { get; set; }
        public string AccountCategoryId { get; set; }
        public string AccountNumberCU { get; set; }
        public bool IsNormalCreation { get; set; }

        public AddAccountCommand()
        {
                
        }
        public AddAccountCommand(   string accountNumber,   string accountName, string accountOwnerId,
    string chartOfAccountManagementPositionId,  string accountNumberManagementPosition,   string ownerBranchCode,  string accountCategoryId,   bool isNormalCreation = true)
        {
            AccountNumber = accountNumber;
            AccountName = accountName;
            AccountOwnerId = accountOwnerId;
            ChartOfAccountManagementPositionId = chartOfAccountManagementPositionId;
            AccountNumberManagementPosition = accountNumberManagementPosition;
            OwnerBranchCode = ownerBranchCode;
            AccountCategoryId = accountCategoryId;
            IsNormalCreation = isNormalCreation;

            // Optional fields set to default/empty values
            AccountTypeId = string.Empty;
            LiaisonBranchCode = string.Empty;
            AccountNumberNetwok = string.Empty;
            AccountNumberCU = string.Empty;
        }
    }

    public class AddAccountCommandDTO : AddAccountCommand
    {

        public string BranchId { get;   set; }
        public string BankId { get;   set; }

        //public static AddAccountCommandDTO CreateAccountwithAccountType(string accountTypeId, UserInfoToken userInfoToken, Data.ChartOfAccount model)
        //{
        //    return new AddAccountCommandDTO
        //    {
        //        BranchId = userInfoToken.BranchId.ToString(),
        //        BankId = userInfoToken.BankId.ToString(),
        //    };
        //}
    }


    public class AddAccountCommanddto : IRequest<ServiceResponse<bool>>
    {
        //public string BookingDirection { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountTypeId { get; set; } = "";
        public string AccountOwnerId { get; set; }
        public string ChartOfAccountId { get; set; }
        public string AccountNumberManagementPosition { get; set; }
        public string BranchCode { get; set; }
        public string AccountNumberNetwok { get; set; }
        public string AccountCategoryId { get; set; }
        public string AccountNumberCU { get; set; }
        public decimal BeginningBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal CreditBalance { get; set; }
        public decimal DebitBalance { get; set; }

    }
}