using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class UpdateCommandDocumentReferenceCode : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }

 
        public string ReferenceCode { get; set; }

 
        public string Description { get; set; }


        public string DocumentId { get; set; }

        public string DocumentTypeId { get; set; }

        public bool IsConditional { get; set; }
    }


}
