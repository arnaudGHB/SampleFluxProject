using CBS.NLoan.Data.Dto.LoanApplicationP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Dto.LoanTermP
{
    public class LoanTermDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int MinInMonth { get; set; }
        public int MaxInMonth { get; set; }
        public List<LoanProductDto> LoanProducts { get; set; }

    }
}
