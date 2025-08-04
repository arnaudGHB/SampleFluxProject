
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBS.CUSTOMER.HELPER.Helper;

namespace CBS.CUSTOMER.DATA.Dto
{
    public class UpdateCustomerDateBirthDto 
    {
        public string? CustomerId { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string? AgeCategoryStatus { get; set; }

    }
}
