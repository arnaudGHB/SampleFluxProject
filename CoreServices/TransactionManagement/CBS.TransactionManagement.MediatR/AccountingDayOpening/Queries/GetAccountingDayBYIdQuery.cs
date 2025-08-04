using CBS.TransactionManagement.Data.Dto.AccountingDayOpening;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.TransactionManagement.Queries.AccountingDayOpening
{
    /// <summary>
    /// Represents a query to retrieve a specific TempAccount by its unique identifier.
    /// </summary>
    //public class GetAccountingDayQuery : IRequest<ServiceResponse<List<AccountingDayDto>>>
    //{
    //    public DateTime Date { get; set; }
    //    public string QueryParameter { get; set; }
    //    public bool ByBranch { get; set; }
    //}
    [DateRangeValidation]
    [BranchRequiredValidation]
    public class GetAccountingDayQuery : IRequest<ServiceResponse<List<AccountingDayDto>>>
    {
        [Required(ErrorMessage = "Date From is required.")]
        public DateTime DateFrom { get; set; }

        [Required(ErrorMessage = "Date To is required.")]
        public DateTime DateTo { get; set; }

        [Required(ErrorMessage = "Query Parameter is required.")]
        public string QueryParameter { get; set; }

        public string BranchId { get; set; }

        public bool ByBranch => QueryParameter == "ByBranch";
    }

    public class DateRangeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (GetAccountingDayQuery)validationContext.ObjectInstance;

            if (model.DateFrom > model.DateTo)
            {
                return new ValidationResult("Date From must not be greater than Date To.");
            }

            return ValidationResult.Success;
        }
    }

    public class BranchRequiredValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (GetAccountingDayQuery)validationContext.ObjectInstance;

            if (model.QueryParameter == "ByBranch" && string.IsNullOrEmpty(model.BranchId))
            {
                return new ValidationResult("Branch is required when querying by branch is selected.");
            }

            return ValidationResult.Success;
        }
    }
}
