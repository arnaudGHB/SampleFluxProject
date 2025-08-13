using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.TransactionManagement.GAV.Queries
{
    public class GetCustomerKYCByAccountNumberQuery : IRequest<ServiceResponse<CustomerKYCDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Transaction to be retrieved.
        /// </summary>
        [Required(ErrorMessage = "Account number is required.")]
        [StringLength(10, MinimumLength = 20, ErrorMessage = "Account number must be 12 to 20 characters long.")]
        public string AccountNumber { get; set; }
    }
}
