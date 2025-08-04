using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{


    public class StatementModelDto
    {
        public string Id { get; set; }
        public string Reference { get; set; }
        public string Heading { get; set; }

        public string Document_type { get; set; }
        public StatementModelDto()
        {
        }
    }
}
