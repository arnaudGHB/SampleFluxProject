using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace CBS.CUSTOMER.DATA.Entity
{
    public class CustomerSmsConfigurations
    {
        [Key]
        public string Id { get; set; }
        public string SmsTemplate { get; set; }

    }
}
