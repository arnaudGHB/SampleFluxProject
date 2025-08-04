using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.OperationEventNameAttributes.Queries
{
    public class UploadOperationEventAttributesQuery : IRequest<ServiceResponse<List<OperationEventAttributesDto>>>
    {
        public List<OperationEventAttributesDto> OperationEventAttributes { get; set; }

    }
}
