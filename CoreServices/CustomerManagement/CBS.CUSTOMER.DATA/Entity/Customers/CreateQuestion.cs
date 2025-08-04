using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA
{
    public class Question:BaseEntity
    {
        public string? SecretQuestion { get; set; }
        [Key]
        public string? QuestionId { get; set; }
    }
}
