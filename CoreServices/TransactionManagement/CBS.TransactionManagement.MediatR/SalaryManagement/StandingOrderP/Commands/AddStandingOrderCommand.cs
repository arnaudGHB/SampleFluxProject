using CBS.TransactionManagement.Data.Dto.SalaryManagement.StandingOrderP;
using CBS.TransactionManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Commands
{
    /// <summary>
    /// Represents a command to add a new ReopenFeeParameter.
    /// </summary>
    public class AddStandingOrderCommand : IRequest<ServiceResponse<StandingOrderDto>>
    {
        public string MemberId { get; set; }
        public decimal Amount { get; set; }
        public string MemberName { get; set; }

        public string SourceAccountType { get; set; }// Salary,Deposit,
        public string DestinationAccountType { get; set; } // Loan, Savings, Deposit, Shares
        public string Purpose { get; set; }//Repay my loan, Cashin to savings, Cashin to ordinary share, Cashin to Preference shares
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsAutomatic { get; set; }
        public string Frequency { get; set; } // Monthly, Weekly, etc.
        public string Priority { get; set; } // High, Medium, Low
        public bool ExternalAccount { get; set; }

        public string? ExternalAccountNumber { get; set; }

        public string? ExternalAccountHolderName { get; set; }
        public string PersonalNote { get; set; }
    }

}
