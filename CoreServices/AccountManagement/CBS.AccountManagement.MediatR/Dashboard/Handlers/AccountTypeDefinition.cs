using CBS.AccountManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class AccountTypeDefinition
    {
        public int Code { get; set; }
        public List<int> CodeArray { get; set; } = new List<int>();
        public bool HasArray { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
    }

}
