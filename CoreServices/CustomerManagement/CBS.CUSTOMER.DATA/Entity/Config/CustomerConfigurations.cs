
using System.ComponentModel.DataAnnotations;


namespace CBS.CUSTOMER.DATA.Entity.Config
{
    public class CustomerConfigurations
    {
        [Key]
        public string ConfigurationId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
