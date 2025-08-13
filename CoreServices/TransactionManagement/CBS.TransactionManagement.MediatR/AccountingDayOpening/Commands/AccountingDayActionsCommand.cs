using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.AccountingDayOpening;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.AccountingDayOpening.Commands
{
    public class AccountingDayActionsCommand : IRequest<ServiceResponse<CloseOrOpenAccountingDayResultDto>>
    {
        public string Id { get; set; }
        public string Option { get; set; }

    }
}
