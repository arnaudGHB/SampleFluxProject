using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class AccountPolicyDto
    {
        public decimal MaximumAlert { get; set; }
        public decimal MinimumAlert { get; set; }
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string MinMessage { get; set; }
        public string PolicyOwner { get; set; }
        public string MaxMessage { get; set; }
        public string Id { get; set; }
    }
}
