using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Entity
{
    public class EmployeeTraining : BaseEntity
    {
        [Key]
        public string? EmployeeTrainingId { get; set; }
        public string? TrainingName { get; set; }
        public DateTime TrainingDate { get; set; }
        public string? TrainingLocation { get; set; }


        public string? EmployeeId { get; set; }

        public Employee? Employee { get; set; }

    }
}
