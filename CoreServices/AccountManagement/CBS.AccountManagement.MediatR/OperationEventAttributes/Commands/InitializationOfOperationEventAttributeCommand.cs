using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class InitializationOfOperationEventAttributeCommand : IRequest<ServiceResponse<List<OperationEventAttributesDto>>>
    {
        public string ServiceOperationTypeId { get; set; }
        public string ServiceOperationType { get; set; }//LOAN,SAVING,REFUND
      
    }
}
