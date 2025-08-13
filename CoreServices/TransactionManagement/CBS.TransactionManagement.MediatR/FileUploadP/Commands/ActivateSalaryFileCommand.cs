using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.FeeMediaR.FileUploadP.Commands
{

   
    public class ActivateSalaryFileCommand : IRequest<ServiceResponse<bool>>
    {
      
        public bool Status { get; set; }
        public string Id { get; set; }
    }

}
