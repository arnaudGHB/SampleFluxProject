using CBS.TransactionManagement.Data.Dto.RemittanceP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Helper.Helper.Pagging;
using MediatR;

namespace CBS.TransactionManagement.MediatR.RemittanceP.Queries
{
    public class GetAllRemittanceQuery : IRequest<ServiceResponse<CustomDataTable>>
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DataTableOptions DataTableOptions { get; set; }
        public bool ByDateRange { get; set; }
        public bool Approved { get; set; }
        public string QueryParameter { get; set; } //Can be By SourceBranchId,ReceivingBranchId or All
        public string QueryValue { get; set; }
        public string BranchId { get; set; }
        public string Status { get; set; }
    }
}
