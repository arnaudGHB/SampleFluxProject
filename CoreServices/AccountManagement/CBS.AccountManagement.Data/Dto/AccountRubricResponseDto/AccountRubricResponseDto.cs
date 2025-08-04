using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class AccountRubricResponseDto
    {

        public string OperationAccountTypeId { get; set; }

        public List<ProductAccountRubric> AccountRubriques { get; set; }
    }
}