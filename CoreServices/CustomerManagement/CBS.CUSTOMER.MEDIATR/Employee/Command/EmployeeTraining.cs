
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.HELPER.Helper;

namespace CBS.Customer.MEDIATR
{
    /// <summary>
    /// Represents a command to add a new EmployeeTraining.
    /// </summary>
    public class AddEmployeeTrainingCommand : IRequest<ServiceResponse<CreateEmployeeTraining>>
    {
        public string? TrainingName { get; set; }
        public DateTime TrainingDate { get; set; }
        public string? TrainingLocation { get; set; }
        public string? EmployeeId { get; set; }



    }

}
