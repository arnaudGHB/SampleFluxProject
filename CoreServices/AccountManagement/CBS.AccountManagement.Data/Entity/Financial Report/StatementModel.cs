using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class StatementModel:BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string Heading { get; set; }
        public string Reference { get; set; }
        public string Document_type { get; set; }
        
    }
}
