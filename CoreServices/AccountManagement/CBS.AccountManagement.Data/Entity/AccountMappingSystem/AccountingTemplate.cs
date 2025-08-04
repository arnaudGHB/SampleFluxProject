using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    public class AccountingTemplate : BaseEntity
    {

        public int TemplateID { get; set; }
        public string TemplateName { get; set; }
        public string TemplateType { get; set; }
        public int NumberOfLegs { get; set; }

        public virtual ICollection<AccountingRuleX> AccountingRules { get; set; }
        public virtual ICollection<TemplateAccountMapping> TemplateAccountMappings { get; set; }
        public virtual ICollection<TemplateContraMapping> TemplateContraMappings { get; set; }

    }

}
