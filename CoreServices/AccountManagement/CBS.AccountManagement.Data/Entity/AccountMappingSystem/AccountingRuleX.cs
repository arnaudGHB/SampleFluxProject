
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class AccountingRuleX : BaseEntity
    {

        public string Id { get; set; }
        public string RuleName { get; set; }
        public string Description { get; set; }
        public string EventType { get; set; }
        public string FormulaParameters { get; set; }
        public string TemplateID { get; set; }
        public virtual ICollection<AccountDetermination> AccountDeterminations { get; set; }
        public virtual ICollection<ContraAccountMapping> ContraAccountMappings { get; set; }
        public virtual EventCatalog EventCatalog { get; set; }
        public virtual AccountingTemplate AccountingTemplate { get; set; }
    }
}
