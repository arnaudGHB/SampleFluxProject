using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data
{
   
 
    public class AccountTypeCreationRequestResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string operationAccountTypeId { get; set; }
        public string operationAccountType { get; set; }

    }

}
