using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class AddCommandDocumentReferenceCode : IRequest<ServiceResponse<bool>>
    {
     
        public string ReferenceCode { get; set; }
        public bool HasException { get; set; }
        public string Description { get; set; }
        public string DocumentId { get; set; }
        public string DocumentTypeId { get; set; }
      
    }
}
