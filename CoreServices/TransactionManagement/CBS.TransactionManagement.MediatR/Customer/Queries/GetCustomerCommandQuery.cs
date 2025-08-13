using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.TransactionManagement.Queries
{
    public class GetCustomerByTelephoneQuery : IRequest<ServiceResponse<CustomerDto>>
    {
        [Required(ErrorMessage = "Telephone number is required.")]
        [RegularExpression(@"^237\d{9}$", ErrorMessage = "Telephone number must start with '237' followed by 9 digits and be 12 characters long.")]
        public string TelephoneNumber { get; set; }
    }
}
