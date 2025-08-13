using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.AccountingDayOpening;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands.AccountingDayOpening
{
    
    public class CloseOfAccountingDayCommand : IRequest<ServiceResponse<List<CloseOrOpenAccountingDayResultDto>>>
    {
        public DateTime Date { get; set; }
        public List<BranchListing>? Branches { get; set; }
        public bool IsCentraliseOpening { get; set; }
    }

}
