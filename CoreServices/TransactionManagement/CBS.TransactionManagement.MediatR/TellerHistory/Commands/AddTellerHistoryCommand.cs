using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new TellerHistory.
    /// </summary>
    public class AddTellerHistoryCommand : IRequest<ServiceResponse<PrimaryTellerProvisioningHistoryDto>>
    {
        public string TellerId { get; set; }
        public string UserId { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public string CurrencyNotesId { get; set; }
        public decimal StartOfDayAmount { get; set; }
        public decimal AccountBalance { get; set; } = 0;

        //all args constructor
        public AddTellerHistoryCommand(string tellerId, string userId, string bankId, string branchId, string currencyNotesId, decimal startOfDayAmount, decimal accountBalance)
        {
            TellerId = tellerId;
            UserId = userId;
            BankId = bankId;
            BranchId = branchId;
            CurrencyNotesId = currencyNotesId;
            StartOfDayAmount = startOfDayAmount;
            AccountBalance = accountBalance;
        }
    }

}
