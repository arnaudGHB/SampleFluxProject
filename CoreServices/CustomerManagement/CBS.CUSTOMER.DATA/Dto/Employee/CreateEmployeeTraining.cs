using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto
{
    public class CreateEmployeeTraining
    {
        public string? TrainingName { get; set; }
        public DateTime TrainingDate { get; set; }
        public string? TrainingLocation { get; set; }
        public string? EmployeeId { get; set; }


    }
}
