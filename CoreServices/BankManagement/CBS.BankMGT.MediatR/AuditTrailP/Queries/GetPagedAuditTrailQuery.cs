using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Data.Pagging;
using CBS.BankMGT.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.MediatR.AuditTrailP.Queries
{
    public class GetPagedAuditTrailQuery : IRequest<ServiceResponse<PaginatedResult<AuditTrailDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
