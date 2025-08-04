using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{

 
    public class DeleteDocumentUploadedCommand : IRequest<ServiceResponse<bool>>
    {
      
        public string Id { get; set; }
    }

}
