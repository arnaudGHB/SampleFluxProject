using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class UpdateDocumentReferenceCodeCommand : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }
        public string Document { get; set; }
        public string DocumentType { get; set; }
        public string ReferenceCode { get; set; }
        public string Description { get; set; }
        public string Interface { get; set; }
        public List<string> GrossCorrespondingAccount { get; set; }

        public List<string> GrossCorrespondingExceptionAccount { get; set; }
        public List<string> ProvCorrespondingExceptionAccount { get; set; }
        public List<string> ProvCorrespondingAccount { get; set; }
    }


}
